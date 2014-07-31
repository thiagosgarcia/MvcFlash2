﻿using System;
using System.Linq;
using System.Web.Mvc;

namespace MvcFlash.Core
{
    public static class Flash
    {
        public static object Sync = new object();
        private static IFlashMessenger _instance;
        internal const string DefaultSuccess = "success";
        internal const string DefaultError = "error";
        internal const string DefaultInfo = "info";
        internal const string DefaulWarning = "attention";
        internal const string DefaultNote = "note";

        public static class Types
        {
            public static string Success = DefaultSuccess;
            public static string Error = DefaultError;
            public static string Info = DefaultInfo;
            public static string Warning = DefaulWarning;
            public static string Note = DefaultNote;
        }

        /// <summary>
        /// Will first check in DependencyResolver.Current for the 
        /// service of type IFlashMessenger, then will check for internal
        /// service set, then will call Initialize if all else fails.
        /// </summary>
        public static IFlashMessenger Instance
        {
            get
            {
                var messenger = DependencyResolver.Current.GetService<IFlashMessenger>();

                if (messenger != null)
                    return messenger;

                if (_instance == null)
                {
                    lock (Sync)
                    {
                        if (_instance == null)
                            Initialize();
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// Use this method to set the Flash Messenger service and also
        /// the defaults for the out of the box message types.
        /// </summary>
        /// <param name="settings"></param>
        public static void Initialize(FlashSettings settings = null)
        {
            if (settings == null)
                settings = FlashSettings.Default;

            if (settings.Types.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("one or more types are empty, please correct", "settings");

            lock (Sync)
            {
                _instance =  DependencyResolver.Current.GetService<IFlashMessenger>()
                          ?? settings.Messenger;
                
                Types.Success = settings.Success;
                Types.Error = settings.Error;
                Types.Info = settings.Info;
                Types.Warning = settings.Warning;
                Types.Note = settings.Note;
            }
        }

        /// <summary>
        /// Call this if you want to reset the types and the
        /// Flash messenger instance to null. Very helpful
        /// for unit testing.
        /// </summary>
        public static void Reset()
        {
            _instance = null;
            Types.Success = DefaultSuccess;
            Types.Error = DefaultError;
            Types.Info = DefaultInfo;
            Types.Warning = DefaulWarning;
            Types.Note = DefaultNote;
        }
    }
}
