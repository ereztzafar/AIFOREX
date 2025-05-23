using System;
using System.Collections.Generic;

namespace AIFOREX
{
    public class PortfolioManager
    {
        private readonly Dictionary<string, Position> _positions = new();

        public void RegisterBuy(string symbol, double price, double quantity)
        {
            if (!_positions.ContainsKey(symbol))
                _positions[symbol] = new Position();

            var pos = _positions[symbol];
            double totalCost = pos.AvgCost * pos.Quantity + price * quantity;
            pos.Quantity += quantity;
            pos.AvgCost = totalCost / pos.Quantity;
        }

        public double RegisterSell(string symbol, double price)
        {
            if (!_positions.ContainsKey(symbol))
                return 0.0;

            var pos = _positions[symbol];
            if (pos.Quantity <= 0)
                return 0.0;

            double profit = (price - pos.AvgCost) * pos.Quantity;
            pos.Quantity = 0;
            pos.AvgCost = 0;
            return profit;
        }

        public bool HasPosition(string symbol)
        {
            return _positions.ContainsKey(symbol) && _positions[symbol].Quantity > 0;
        }

        public double GetAvgCost(string symbol)
        {
            return _positions.ContainsKey(symbol) ? _positions[symbol].AvgCost : 0.0;
        }

        public double GetQuantity(string symbol)
        {
            return _positions.ContainsKey(symbol) ? _positions[symbol].Quantity : 0.0;
        }
    }

    public class Position
    {
        public double Quantity { get; set; } = 0.0;
        public double AvgCost { get; set; } = 0.0;
    }
}
