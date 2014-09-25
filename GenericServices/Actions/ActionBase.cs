#region licence
// The MIT License (MIT)
// 
// Filename: ActionBase.cs
// Date Created: 2014/05/27
// 
// Copyright (c) 2014 Jon Smith (www.selectiveanalytics.com & www.thereformedprogrammer.net)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion
using System;
using GenericServices.ActionComms;
using GenericServices.Logger;

namespace GenericServices.Actions
{
    public abstract class ActionBase
    {

        private readonly IGenericLogger _logger;

        /// <summary>
        /// Override this to return true if you want the calling service to write to database.
        /// If the method exited with status IsValid and the warning check (see ICheckIfWarings)
        /// passes the it will call SubmitChanges to commit any data to the database
        /// </summary>
        public virtual bool SubmitChangesOnSuccess { get { return false; } }
      
        /// <summary>
        /// This controls the lower value sent back to reportProgress
        /// Lower and Upper bound are there to allow outer tasks to call inner tasks 
        /// to do part of the work and still report progress properly
        /// </summary>
        public int LowerBound { get; set; }

        /// <summary>
        /// This controls the upper bound of the value sent back to reportProgress
        /// </summary>
        public int UpperBound { get; set; }

        protected ActionBase()
        {
            LowerBound = 0;
            UpperBound = 100;
            _logger = ServicesConfiguration.GetLogger(GetType().Name);       //give it the name of the inherited type
        }

        
        //---------------------------------------------------
        //private helpers

        protected void SendtoLogger(ProgressMessage message)
        {

            switch (message.MessageType)
            {
                case ProgressMessageTypes.Notset:
                    break;
                case ProgressMessageTypes.Verbose:
                    _logger.Verbose(message.MessageText);
                    break;
                case ProgressMessageTypes.Info:
                    _logger.Info(message.MessageText);
                    break;
                case ProgressMessageTypes.Warning:
                    _logger.Warn(message.MessageText);
                    break;
                case ProgressMessageTypes.Error:
                    _logger.Error(message.MessageText);
                    break;
                case ProgressMessageTypes.Critical:
                    _logger.Critical(message.MessageText);
                    break;
                case ProgressMessageTypes.Finished:
                    _logger.InfoFormat("Finished: {0}", message.MessageText);
                    break;
                case ProgressMessageTypes.Cancelled:
                    _logger.InfoFormat("Cancelled: {0}", message.MessageText);
                    break;
                case ProgressMessageTypes.Failed:
                    _logger.InfoFormat("FAILED: {0}", message.MessageText);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}
