﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Darkness.Cqrs.Simple
{
    public class QueryBus : IQueryBus
    {
        private IQueryHandlerResolver Resolver { get; }

        public QueryBus(IQueryHandlerResolver resolver)
        {
            Resolver = resolver;
        }

        public TResult Ask<TResult>(IQuery<TResult> query)
        {
            if (query == null) 
                throw new ArgumentNullException(nameof(query));
            
            var queryType = query.GetType();

            var handler = Resolver.Resolve(typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResult)));

            var method = handler.GetType().GetMethod("Handle", new[] {query.GetType()});

            return (TResult) method.Invoke(handler, new object[] {query});
        }

        public Task<TResult> AskAsync<TResult>(IQuery<TResult> query, CancellationToken token = default(CancellationToken))
        {
            if (query == null) 
                throw new ArgumentNullException(nameof(query));
            
            var queryType = query.GetType();

            var handler = Resolver.Resolve(typeof(IQueryHandlerAsync<,>).MakeGenericType(queryType, typeof(TResult)));

            var method = handler.GetType().GetMethod("Handle", new[] {query.GetType(), token.GetType()});

            return (Task<TResult>) method.Invoke(handler, new object[] {query, token});
        }
        
    }
}