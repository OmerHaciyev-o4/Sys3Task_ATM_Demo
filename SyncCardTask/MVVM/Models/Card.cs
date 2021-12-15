using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace SyncCardTask.MVVM.Models
{ 
    public class Card
    {
        private string _firstname { get; set; }
        private string _lastname { get; set; }
        private long _cardCode { get; set; }
        private double _amount { get; set; }


        public Card(string Firstname, string Lastname, long CardCode, double Amount = 0)
        {
            _firstname = Firstname;
            _lastname = Lastname;
            _cardCode = CardCode;
            _amount = Amount;
        }

        public long GetCardCode() => _cardCode;

        public void SetAmount(double amount) => _amount -= amount;

        public string GetFullname() => $"{_firstname} {_lastname}";
        public double GetAmount() => _amount;
    }  
} 