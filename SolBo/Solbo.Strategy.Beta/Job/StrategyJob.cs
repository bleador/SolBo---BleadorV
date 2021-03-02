﻿using Quartz;
using Solbo.Strategy.Beta.Models;
using Solbo.Strategy.Beta.Rules;
using Solbo.Strategy.Beta.Verificators.Strategy;
using SolBo.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Solbo.Strategy.Beta.Job
{
    [DisallowConcurrentExecution]
    public class StrategyJob : IJob
    {
        private readonly IFileService _fileService;
        private readonly ILoggingService _loggingService;

        private ICollection<IBetaRules> _rules;
        public StrategyJob(
            IFileService fileService,
            ILoggingService loggingService)
        {
            _fileService = fileService;
            _loggingService = loggingService;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            _rules = new HashSet<IBetaRules>();
            try
            {
                var strategyName = context.JobDetail.JobDataMap["name"] as string;
                var path = context.JobDetail.JobDataMap["path"] as string;
                var jobArgs = await _fileService.DeserializeAsync<StrategyRootModel>(path);
                var symbol = context.JobDetail.JobDataMap["symbol"] as string;
                var jobPerSymbol = jobArgs.Pairs.FirstOrDefault(j => j.Symbol == symbol);

                if (jobPerSymbol is null)
                    return;

                _rules.Add(new StrategyModelVerificator());

                _loggingService.Info($"{context.JobDetail.Key.Name} - START JOB - TASKS ({_rules.Count})");

                foreach (var item in _rules)
                {
                    var result = item.Result(jobPerSymbol);
                    if (!result.Success)
                    {
                        _loggingService.Error($"{context.JobDetail.Key.Name}|{Environment.NewLine}{result.Message}");
                        break;
                    }
                    else
                    {
                        _loggingService.Info($"{context.JobDetail.Key.Name} - PROCEED TASK");
                    }
                }

                _loggingService.Info($"{context.JobDetail.Key.Name} - END JOB - TASKS ({_rules.Count})");
            }
            catch (JobExecutionException e)
            {
                _loggingService.Error($"{context.JobDetail.Key.Name}|{Environment.NewLine}Message => {e.Message}{Environment.NewLine}StackTrace => {e.StackTrace}");
            }
        }
    }
}