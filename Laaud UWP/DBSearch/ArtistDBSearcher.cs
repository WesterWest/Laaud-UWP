﻿using Laaud_UWP.Models;
using Laaud_UWP.Util;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laaud_UWP.DBSearch
{
    class ArtistDBSearcher : DBSearcher<Artist, string>
    {
        private readonly AlbumDBSearcher albumDBSearcher;

        public ArtistDBSearcher()
        {
            this.albumDBSearcher = null;
        }

        protected override List<Artist> GetByPrimaryKeys(List<int> primaryKeyCollection)
        {
            using (MusicLibraryContext dbContext = new MusicLibraryContext())
            {
                List<Artist> artists = dbContext.Artists
                    .Include(artist => artist.Albums)
                    .Include("Albums.Songs")
                    .Where(artist => primaryKeyCollection.Contains(artist.ArtistId))
                    .ToList();

                foreach (Artist artist in artists)
                {
                    artist.Albums = artist.Albums.OrderBy(album => album.Name).ToList();
                }

                return artists;
            }
        }

        protected override List<int> GetItemIdsForSearchTerms(string searchTerm)
        {
            using (MusicLibraryContext dbContext = new MusicLibraryContext())
            {
                return dbContext.Artists
                    .Where(
                        artist => artist.Name.ContainsIgnoreCase(searchTerm))
                    .OrderBy(artist => artist.Name)
                    .Select(artist => artist.ArtistId)
                    .ToList();
            }
        }

        protected override int GetPrimaryKey(Artist loadedItem)
        {
            return loadedItem.ArtistId;
        }
    }
}
