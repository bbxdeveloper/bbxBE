using bbxBE.Application.Exceptions;
using bbxBE.Application.Wrappers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace bbxBE.WebApi.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        //  private readonly ILogger _logger;

//        public ErrorHandlerMiddleware(RequestDelegate next, ILogger p_Logger)
        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
//            _logger = p_Logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                var responseModel = new Response<string>() { Succeeded = false, Message = error?.Message };

                switch (error)
                {
                    case Application.Exceptions.ApiException e:
                        // custom application error
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;

                    case ValidationException e:
                        // custom application error
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        responseModel.Errors = e.Errors;
                        break;

                    case KeyNotFoundException e:
                        // not found error
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        responseModel.Errors = new List<string>();
                        responseModel.Errors.Add(e.Message);
                        break;

                    case ResourceNotFoundException e:
                        // not found error
                        //response.StatusCode = (int)HttpStatusCode.NoContent;
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        responseModel.Errors = new List<string>();
                        responseModel.Errors.Add(e.Message);
                        break;

                    case ImportParseException e:
                        // parsing problem
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        responseModel.Errors = new List<string>();
                        responseModel.Errors.Add(e.Message);
                        break;

                    case LockedCacheException e:
                        // parsing problem
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;

                    case InvalidOperationException e:
                        if (e.InnerException != null && e.InnerException.GetType() == typeof(AggregateException))
                        {
                            response.StatusCode = (int)HttpStatusCode.BadRequest;

                            var ae = e.InnerException as AggregateException;
                            if (ae.Flatten().InnerExceptions.Count == 1)
                            {
                                //Ha csak egy inner exception akkor Message-ba berakjuk az InnerException-t
                                //responseModel.Errors.Add(responseModel.Message);
                                responseModel.Message = ae.Flatten().InnerExceptions.First().Message;
                            }
                            else
                            {
                                responseModel.Errors = new List<string>();
                                foreach (var ie in ae.Flatten().InnerExceptions)
                                {
                                    responseModel.Errors.Add(ie.Message);
                                }
                            }
                        }
                        else
                        {
                            response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        }
                        break;

                    default:
                        // unhandled error
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }
                var result = JsonConvert.SerializeObject(responseModel);

  //              _logger.LogError(error, error.Message);
                await response.WriteAsync(result);
                throw;
            }
        }
    }
}