#region licence
// The MIT License (MIT)
// 
// Filename: IActionService.cs
// Date Created: 2014/05/26
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

using GenericLibsBase;
using GenericServices.Core;

namespace GenericServices
{
    public interface IActionService<TActionOut, in TActionIn>
    {
        /// <summary>
        /// This runs a method, handing it the data it needs
        /// </summary>
        /// <param name="taskData"></param>
        /// <returns>The status, with a result if the status is valid</returns>
        ISuccessOrErrors<TActionOut> DoAction(TActionIn taskData);
    }

    public interface IActionService<TActionOut, TActionIn, TDto>
        where TActionIn : class, new()
        where TDto : EfGenericDto<TActionIn, TDto>, new()
    {
        /// <summary>
        /// This converts the dto to the format that the method needs and then runs it
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>The status, with a result if the status is valid</returns>
        ISuccessOrErrors<TActionOut> DoAction(TDto dto);

        /// <summary>
        /// This is available to reset any secondary data in the dto. Call this if the ModelState was invalid and
        /// you need to display the view again with errors
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        TDto ResetDto(TDto dto);
    }
}