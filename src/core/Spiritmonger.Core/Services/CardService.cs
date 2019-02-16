using AutoMapper;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Spiritmonger.Core.Contracts.DTO;
using Spiritmonger.Core.Contracts.Services;
using Spiritmonger.Domain.Entities;
using Spiritmonger.Persistence.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spiritmonger.Core.Services
{
    public class CardService : BaseService<Card, CardDTO>, ICardService
    {
        private readonly ISqlContextProvider _contextProvider;

        public CardService(ISqlContextProvider contextProvider) : base(contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public async Task<(IList<CardDTO> response, int totalCount)> SearchAsync(string searchParam, int? skip = null, int? take = null, bool relevance = false)
        {
            if (relevance)
                return await GetRelevanceIndexedSearchAsync(searchParam, skip, take);
            else
                return await GetUnsortedSearchAsync(searchParam, skip, take);
        }


        private async Task<(IList<CardDTO> response, int totalCount)> GetRelevanceIndexedSearchAsync(string searchParam, int? skip = null, int? take = null)
        {
            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };
            var result = FilterProducts(searchParam).ToAsyncEnumerable().OrderBy(card =>
            {
                var searchParams = searchParam.Contains(" ") && !searchParam.Contains(",") ? searchParam.Split(" ") : new[] { searchParam };
                var score = 0;
                Parallel.For(0, searchParams.Length, parallelOptions, index =>
                {
                    var multiplier = 1;
                    var current = (card.Name.ToLower().IndexOf(searchParams[index]) + 1) * multiplier;
                    if (current <= 0)
                    {
                        multiplier *= 50;
                        current = score > -1 ? (int)(multiplier * (index + 1)) : -1;
                    }
                    score += current;
                });
                score = score < 0 ? int.MaxValue : score;
                card.Relevance = score;
                return score;
            }).ThenBy(p => p.Name).ThenBy(p => p.Expansion).AsAsyncEnumerable();

            var totalCount = await result.Count();

            if (take > 0)
            {
                skip = skip ?? 0;
                result = result
                    .Skip((int)skip)
                    .Take((int)take);
            }
            else if (take == -1)
            {
                result = result.Skip(skip ?? 0);
            }
            else
            {
                result = result.Take(12);
            }

            return (result.Select(o => Mapper.Map<CardDTO>(o)).ToEnumerable().ToList(), totalCount);
        }

        private async Task<(IList<CardDTO> response, int totalCount)> GetUnsortedSearchAsync(string searchParam, int? skip = null, int? take = null)
        {
            var result = FilterProducts(searchParam);
            var totalCount = await result.CountAsync();

            if (take > 0)
            {
                skip = skip ?? 0;
                result = result
                    .Skip((int)skip)
                    .Take((int)take);
            }
            else if (take == -1)
            {
                result = result.Skip(skip ?? 0);
            }
            else
            {
                result = result.Take(12);
            }
            return (await result.Select(o => Mapper.Map<CardDTO>(o)).ToListAsync(), totalCount);
        }



        private IQueryable<Card> FilterProducts(string searchParam)
        {
            if (!string.IsNullOrEmpty(searchParam))
            {
                string[] searchParams;
                var predicate = PredicateBuilder.New<Card>();

                if (searchParam.Contains(" ") && !searchParam.Contains(","))
                {
                    searchParams = searchParam.Split(" ");
                    foreach (var par in searchParams)
                    {
                        var innerPredicate = PredicateBuilder.New<Card>()
                            .Or(p => p.Name.Contains(par));
                            //.Or(p => p.Expansion.Contains(par));

                        predicate.And(innerPredicate);
                    }
                }
                else
                {
                    predicate = PredicateBuilder.New<Card>()
                        .Or(p => p.Name.Contains(searchParam));
                        //.Or(p => p.Expansion.Contains(searchParam));
                }

                return _contextProvider
                        .Context.Set<Card>()
                        .Where(predicate)
                        .AsExpandable();
            }

            return _contextProvider
                    .Context.Set<Card>()
                    .AsExpandable();
        }
    }
}
