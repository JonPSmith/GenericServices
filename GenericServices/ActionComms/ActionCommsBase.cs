#region licence
// The MIT License (MIT)
// 
// Filename: ActionCommsBase.cs
// Date Created: 2014/07/10
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
using GenericServices.Actions;

namespace GenericServices.ActionComms
{
    public abstract class ActionCommsBase : ActionBase
    {

        private int _lastReportedProgressPercentage = -1;


        /// <summary>
        /// This allows the action to configure what it supports, which then affects what the user sees
        /// Note: it must be a constant as it is read just after the action is created
        /// </summary>
        public abstract ActionFlags ActionConfig { get; }

        /// <summary>
        /// This reports progress with optional message.
        /// After sending the message it checks for cancellation and throws OperationCanceledException if pending
        /// </summary>
        /// <param name="actionComms">The comms channel for handling progress and cancellation</param>
        /// <param name="percentageDone">must be between 0 and 100</param>
        /// <param name="message">optional message to show user</param>
        protected void ReportProgressAndCheckCancel(IActionComms actionComms, int percentageDone, ProgressMessage message = null)
        {
            if (actionComms == null) return;

            ReportProgress(actionComms, percentageDone, message);
            actionComms.ThrowExceptionIfCancelPending();
        }

        /// <summary>
        /// This reports progress with optional message.
        /// </summary>
        /// <param name="actionComms">The comms channel for handling progress and cancellation</param>
        /// <param name="percentageDone">must be between 0 and 100</param>
        /// <param name="message">optional message to show user</param>
        protected void ReportProgress(IActionComms actionComms, int percentageDone, ProgressMessage message = null)
        {
            if (actionComms == null) return;

            var percentageToReport = (int)(LowerBound + ((Math.Min(percentageDone, 100) / 100.0)) * (UpperBound - LowerBound));
            if (percentageToReport != _lastReportedProgressPercentage || message != null)
                //we only report progess if the progress percent has changed or there is a message to send
                actionComms.ReportProgress(percentageToReport, message);

            if (message != null)
                SendtoLogger(message);

            _lastReportedProgressPercentage = percentageToReport;
        }

        /// <summary>
        /// This will throw an OperationCanceledException if the user has asked for the task to be cancelled.
        /// This is the recommended way of checking for cancellation
        /// <param name="actionComms">The comms channel for handling progress and cancellation</param>
        /// </summary>
        protected void ThrowExceptionIfCancelPending(IActionComms actionComms)
        {
            if (actionComms != null)
                actionComms.ThrowExceptionIfCancelPending();
        }

        /// <summary>
        /// Returns true if user has asked for cancellation
        /// </summary>
        /// <param name="actionComms">The comms channel for handling progress and cancellation</param>
        /// <returns>Returns true if user has asked for cancellation</returns>
        protected bool CancelPending(IActionComms actionComms)
        {
            return actionComms != null && actionComms.CancellationPending;
        }

    }
}
