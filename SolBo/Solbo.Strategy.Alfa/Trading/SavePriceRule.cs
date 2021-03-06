﻿using Solbo.Strategy.Alfa.Models;
using Solbo.Strategy.Alfa.Rules;
using SolBo.Shared.Domain.Statics;
using SolBo.Shared.Extensions;
using SolBo.Shared.Services;
using SolBo.Shared.Strategies.Predefined.Results;
using System;

namespace Solbo.Strategy.Alfa.Trading
{
    public class SavePriceRule : IAlfaRule
    {
        private readonly IFileService _fileService;
        private readonly string _strategy;
        public SavePriceRule(
            IFileService fileService,
            string strategy)
        {
            _fileService = fileService;
            _strategy = strategy;
        }
        public IRuleResult Result(StrategyModel strategyModel)
        {
            var errors = string.Empty;
            try
            {
                _fileService.SaveValue(
                    GlobalConfig.PriceFile(_strategy, strategyModel.Symbol),
                    strategyModel.Communication.CurrentPrice.GetValueOrDefault());
            }
            catch (Exception ex)
            {
                errors += ex.GetFullMessage();
            }
            return new RuleResult(errors);
        }
    }
}