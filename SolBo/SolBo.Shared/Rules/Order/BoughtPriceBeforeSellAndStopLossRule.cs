﻿using SolBo.Shared.Domain.Configs;

namespace SolBo.Shared.Rules.Order
{
    public class BoughtPriceBeforeSellAndStopLossRule : IOrderRule
    {
        public string OrderStep => "BoughtPriceBeforeSellAndStopLoss";
        public IRuleResult RuleExecuted(Solbot solbot)
        {
            var response = solbot.Actions.BoughtPrice > 0;
            return new OrderRuleResult
            {
                Success = response,
                Message = response
                    ? "You are able to sell, bought before"
                    : "You are not able to sell, not bought before"
            };
        }
    }
}