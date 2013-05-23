using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI;

namespace Treasury
{
    class TreasuryAccount
    {
        /// <summary>
        /// The TSPlayer that this account reflects
        /// </summary>
        public TSPlayer Player { get; set; }

        /// <summary>
        /// A Bank Name organized list of accounts that belong to this players user account
        /// </summary>
        public Dictionary<string, int> Accounts { get; set; }
 
        public TreasuryAccount(TSPlayer ply)
        {
            Accounts = new Dictionary<string, int>();
        }

        /// <summary>
        /// Adds or updates the account for the bank
        /// </summary>
        /// <param name="bank_name"></param>
        /// <param name="amount"></param>
        public void AddOrUpdateAccount(string bank_name, int amount)
        {
            if(Accounts.ContainsKey(bank_name))
            {
                Accounts[bank_name] = amount;
            }
            else
            {
                Accounts.Add(bank_name, amount);
            }

        }

        /// <summary>
        /// Returns the amount in the bank for this account
        /// </summary>
        /// <param name="bank_name"></param>
        /// <returns></returns>
        public int GetAccount(string bank_name)
        {
            if (Accounts.ContainsKey(bank_name))
                return Accounts[bank_name];

            return 0;
        }
    }
}
