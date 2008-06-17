﻿/**
 * GBlogSearcher.cs
 *
 * Copyright (C) 2008,  iron9light
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Net;

namespace Google.API.Search
{
    public static class GBlogSearcher
    {
        private static int s_Timeout = 0;

        /// <summary>
        /// Get or set the length of time, in milliseconds, before the request times out.
        /// </summary>
        public static int Timeout
        {
            get
            {
                return s_Timeout;
            }
            set
            {
                if (s_Timeout < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                s_Timeout = value;
            }
        }

        internal static SearchData<GBlogResult> GSearch(string keyword, int start, ResultSizeEnum resultSize, SortType sortBy)
        {
            if (keyword == null)
            {
                throw new ArgumentNullException("keyword");
            }

            GBlogSearchRequest request = new GBlogSearchRequest(keyword, start, resultSize, sortBy);

            WebRequest webRequest;
            if (Timeout != 0)
            {
                webRequest = request.GetWebRequest(Timeout);
            }
            else
            {
                webRequest = request.GetWebRequest();
            }

            SearchData<GBlogResult> responseData;
            try
            {
                responseData = RequestUtility.GetResponseData<SearchData<GBlogResult>>(webRequest);
            }
            catch (GoogleAPIException ex)
            {
                throw new SearchException(string.Format("request:\"{0}\"", request), ex);
            }
            return responseData;
        }

        public static IList<IBlogResult> Search(string keyword, int resultCount)
        {
            return Search(keyword, resultCount, new SortType());
        }

        public static IList<IBlogResult> Search(string keyword, int resultCount, SortType sortBy)
        {
            if(keyword == null)
            {
                throw new ArgumentNullException("keyword");
            }

            SearchUtility.GSearchCallback<GBlogResult> gsearch = (start, resultSize) => GSearch(keyword, start, resultSize, sortBy);
            List<GBlogResult> results = SearchUtility.Search(gsearch, resultCount);
            return results.ConvertAll<IBlogResult>(item => (IBlogResult)item);
        }
    }
}