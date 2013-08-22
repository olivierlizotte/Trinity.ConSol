using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alchemy.Classes;

namespace Afinity
{
    public class ConSol
    {
        /// <summary>
        /// Store the list of online users. Wish I had a ConcurrentList. 
        /// </summary>
        protected ConcurrentDictionary<string, ConSolUser> OnlineUsersByName = new ConcurrentDictionary<string, ConSolUser>();

        /// <summary>
        /// Store the list of online users. Wish I had a ConcurrentList. 
        /// </summary>
        protected ConcurrentDictionary<UserContext, ConSolUser> OnlineUsersByContext = new ConcurrentDictionary<UserContext, ConSolUser>();

        /// <summary>
        /// Store the list of online users. Wish I had a ConcurrentList. 
        /// </summary>
        protected Logger Logs;
        public ConSol()
        {
            Logs = new Logger();
        }

        public void WriteLine(string msg)
        {
            foreach(ConSolUser u in OnlineUsersByName.Values)//TODO for debugging purposes, send outputs everywhere
                u.WriteLine(msg);
            Console.WriteLine(msg);
        }

        public ConSolUser CreateUserConSol(UserContext context, string name)
        {
            if (OnlineUsersByName.ContainsKey(name))
                return OnlineUsersByName[name];
            else
            {
                ConSolUser user = new ConSolUser(context, name, Logs);
                OnlineUsersByName.TryAdd(name, user);
                OnlineUsersByContext.TryAdd(context, user);
                return user;
            }
        }

        public ConSolUser RemoveUserConSol(UserContext context)
        {
            try
            {
                if (OnlineUsersByContext.ContainsKey(context))
                {
                    ConSolUser user = OnlineUsersByContext[context];
                    OnlineUsersByContext.TryRemove(context, out user);
                    OnlineUsersByName.TryRemove(user.Name, out user);
                    return user;
                }
            }
            catch (Exception ex)
            {
                WriteLine(ex.Message);
                WriteLine(ex.StackTrace);
            }
            return null;
        }

        public void UpdateLogFile()
        {
            Logs.UpdateLogFile();
        }
    }

    public class ConSolUser
    {
        private Logger Logs;
        public string Name = String.Empty;
        public UserContext Context { get; set; }

        public ConSolUser(UserContext userContext, string name, Logger log)
        {
            this.Context = userContext;
            this.Name = name;
            this.Logs = log;
        }

        public void WriteLine(string msg)
        {
            Logs.Add(msg, Name);
            Context.Send(msg);
        }
    }
}
