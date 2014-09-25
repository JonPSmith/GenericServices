#region licence
// The MIT License (MIT)
// 
// Filename: IActionServiceAsync.cs
// Date Created: 2014/06/25
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
using System.Threading.Tasks;
using GenericServices.Core;

namespace GenericServices.ServicesAsync
{
    public interface IActionServiceAsync<TActionOut, in TActionIn>
    {
        /// <summary>
        /// This runs a action that returns a result. 
        /// </summary>
        /// <param name="actionData">Data that the action takes in to undertake the action</param>
        /// <returns>The Task status, with a result if Valid</returns>
        Task<ISuccessOrErrors<TActionOut>> DoActionAsync(TActionIn actionData);
    }
}

namespace GenericServices.ServicesAsync
{
    public interface IActionServiceAsync<TActionOut, TActionIn, TDto> 
        where TActionIn : class, new()
        where TDto : EfGenericDtoAsync<TActionIn, TDto>, new()
    {
        /// <summary>
        /// This runs an action that does not write to the database. 
        /// It first converts the dto to the TActionIn format and then runs the action
        /// </summary>
        /// <param name="dto">The dto to be converted to the TActionIn class</param>
        /// <returns>The Task status, with a result if the status is valid</returns>
        Task<ISuccessOrErrors<TActionOut>> DoActionAsync(TDto dto);

        /// <summary>
        /// This is available to reset any secondary data in the dto. Call this if the ModelState was invalid and
        /// you need to display the view again with errors
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<TDto> ResetDtoAsync(TDto dto);
    }
}