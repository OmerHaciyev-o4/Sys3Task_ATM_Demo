using SyncCardTask.MVVM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SyncCardTask.MVVM.Commands;

namespace SyncCardTask.MVVM.ViewModels
{
    class MainViewModel : BaseViewModel
    {
        #region Private Variable

        private List<Card> _cards;
        private int _id;
        private static object _loadObj;
        private static object _trObj;

        #endregion

        #region Commands

        public ICommand InsertCardCommand { get; set; }
        public ICommand LoadInfoCommand { get; set; }
        public ICommand TransferButtonCommand { get; set; }

        #endregion

        #region Public Variable

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Full Property

        private bool cardCodeEnabled;
        public bool CardCodeEnabled
        {
            get { return cardCodeEnabled; }
            set { cardCodeEnabled = value; OnPropertyChanged(); }
        }

        private bool loadButtonEnabled;
        public bool LoadButtonEnabled
        {
            get { return loadButtonEnabled; }
            set { loadButtonEnabled = value; OnPropertyChanged(); }
        }

        private bool moneyTextBoxEnabled;
        public bool MoneyTextBoxEnabled
        {
            get { return moneyTextBoxEnabled; }
            set { moneyTextBoxEnabled = value; OnPropertyChanged(); }
        }

        private bool transferButtonEnabled;
        public bool TransferButtonEnabled
        {
            get { return transferButtonEnabled; }
            set { transferButtonEnabled = value; OnPropertyChanged(); }
        }



        private string cardCode;
        public string CardCode
        {
            get { return cardCode; }
            set { cardCode = value; onPropertyChanged_CardCodeTextChanged(); OnPropertyChanged(); }
        }

        private string userInfo;
        public string UserInfo
        {
            get { return userInfo; }
            set { userInfo = value; OnPropertyChanged(); }
        }

        private string currentMoney;
        public string CurrentMoney
        {
            get { return currentMoney; }
            set { currentMoney = value; OnPropertyChanged(); }
        }

        private string transferMoney;
        public string TransferMoney
        {
            get { return transferMoney; }
            set { transferMoney = value; onPropertyChanged_TransferMoneyTextChanged(); OnPropertyChanged(); }
        }

        private string resultPrice;
        public string ResultPrice
        {
            get { return resultPrice; }
            set
            {
                resultPrice = value;
                OnPropertyChanged();
            }
        }
        
        #endregion


        public MainViewModel()
        {
            #region Set new data

            _cards = new List<Card>();
            _cards.Add(new Card("Omer", "Haciyev", 4444000044440000, 118));
            _cards.Add(new Card("Hesen", "Hesenli", 12345678945, 864.9));
            _cards.Add(new Card("Jhon", "Jhon-lu", 987456123089, 4537.9));

            _loadObj = new object();
            _trObj = new object();
            LoadButtonEnabled = false;

            #endregion

            #region Commands

            InsertCardCommand = new RelayCommand((o) =>
            {
                if (CardCodeEnabled == false)
                    CardCodeEnabled = true;
                else if (CardCodeEnabled == true)
                {
                    CardCode = "";
                    UserInfo = "";
                    CurrentMoney = "";
                    TransferMoney = "";
                    ResultPrice = "";

                    CardCodeEnabled = false;
                    MoneyTextBoxEnabled = false;
                    TransferButtonEnabled = false;
                }
            });

            LoadInfoCommand = new RelayCommand((o) =>
            {
                lock (_loadObj)
                {
                    if (CheckCard(CardCode))
                    {
                        _id = GetById(CardCode);

                        UserInfo = _cards[_id].GetFullname();
                        CurrentMoney = _cards[_id].GetAmount().ToString() + " AZN";

                        MoneyTextBoxEnabled = true;
                    }
                    else
                    {
                        MoneyTextBoxEnabled = false;
                        TransferMoney = "";

                        MessageBox.Show("Please enter correct Card Code.", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            });

            TransferButtonCommand = new RelayCommand((o) => { ThreadPool.QueueUserWorkItem((e) => TransferWork()); });

            #endregion
        }

        private void TransferWork()
        {
            lock (_trObj)
            {
                if (TransferMoney != "")
                {
                    double money = Convert.ToDouble(TransferMoney);
                    double tempMoney = money;
                    int count = Match(money);
                    double currectMoney = 0;
                    ResultPrice = "0";

                    while (count > 0)
                    {
                        count--;
                        money -= 10;
                        currectMoney += 10;
                        TransferMoney = money.ToString();
                        ResultPrice = currectMoney.ToString();

                        Thread.Sleep(1000);
                    }

                    currectMoney += money;
                    ResultPrice = currectMoney.ToString();
                    TransferMoney = "";

                    Thread.Sleep(1000);
                    _cards[_id].SetAmount(tempMoney);

                    if (_cards[_id].GetAmount() == 0)
                        CurrentMoney = "0 AZN";
                    else
                        CurrentMoney = _cards[_id].GetAmount().ToString() + " AZN";

                    MessageBox.Show("Transfer successfully", "Information", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                    MessageBox.Show("Please enter TransferMoney", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private int Match(double money)
        {
            int count = 0;

            double tempNum = money / 10;
            count = (int) tempNum;
            return count;
        }

        private int GetById(string cardCode)
        {
            long code = Convert.ToInt64(cardCode);
            int id = -1;
            for (int i = 0; i < _cards.Count; i++)
            {
                if (code == _cards[i].GetCardCode())
                {
                    id = i;
                    break;
                }
            }
            return id;
        }

        private bool CheckCard(string cardCode)
        {
            bool state = false;
            if (cardCode != "")
            {
                long code = Convert.ToInt64(cardCode);

                foreach (var card in _cards)
                {
                    if (code == card.GetCardCode())
                    {
                        state = true;
                        break;
                    }
                }
            }

            return state;
        }

        protected void onPropertyChanged_CardCodeTextChanged([CallerMemberName] string name = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }

            if (CardCode.Length > 0)
            {
                for (int i = 0; i < CardCode.Length; i++)
                {
                    if (!Regex.IsMatch(CardCode[i].ToString(), "^[0-9]"))
                        CardCode = CardCode.Remove(i, 1);
                }

                if (CardCode.Length >= 10)
                    LoadButtonEnabled = true;
                else
                    LoadButtonEnabled = false;
            }
            else
                LoadButtonEnabled = false;
        }

        protected void onPropertyChanged_TransferMoneyTextChanged([CallerMemberName] string name = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }

            if (TransferMoney.Length > 0)
            {
                for (int i = 0; i < TransferMoney.Length; i++)
                {
                    if (!Regex.IsMatch(TransferMoney[i].ToString(), "^[0-9]"))
                        TransferMoney = TransferMoney.Remove(i, 1);
                }

                if (TransferMoney != "")
                {
                    string currentMoneyTemp = CurrentMoney;
                    currentMoneyTemp = currentMoney.Remove(currentMoneyTemp.Length - 4, 4);
                    double trMoney = Convert.ToDouble(TransferMoney);
                    double crMoney = Convert.ToDouble(currentMoneyTemp);

                    if (trMoney <= crMoney)
                        TransferButtonEnabled = true;
                    else
                        TransferButtonEnabled = false;
                }
                else TransferButtonEnabled = false;
            }
            else
                TransferButtonEnabled = false;
        }
    }
}