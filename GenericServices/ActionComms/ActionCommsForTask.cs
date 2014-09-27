#region licence
// The MIT License (MIT)
// 
// Filename: ActionCommsForTask.cs
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
using System.Threading;

namespace GenericServices.ActionComms
{
    public class ActionCommsForTask : IActionComms
    {
        private readonly CancellationToken _ctx;
        private readonly IProgress<ProgressWithOptionalMessage> _progressReporter;

        /// <summary>
        /// This is true if the user has asked for the task to cancel
        /// </summary>
        public bool CancellationPending { get { return _ctx.IsCancellationRequested; } }

        /// <summary>
        /// This will throw an OperationCanceledException if the user has asked for the task to be cancelled.
        /// This is the recommended way of checking for cancellation
        /// </summary>
        public void ThrowExceptionIfCancelPending()
        {
            _ctx.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// This sends a status update to the user from the running task
        /// </summary>
        /// <param name="percentageDone">goes from 0 to 100</param>
        /// <param name="message">message, with message type in. Can be null for no message</param>
        public void ReportProgress(int percentageDone, ProgressMessage message = null)
        {
            _progressReporter.Report( new ProgressWithOptionalMessage(percentageDone, message));
        }

        public ActionCommsForTask(CancellationToken ctx, IProgress<ProgressWithOptionalMessage> progressReporter)
        {
            _ctx = ctx;
            _progressReporter = progressReporter;
        }
    }
}
