using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace Brandmark
{
    class Library
    {
        public static string bookmarkLoc = null;
        private static string bookmarkContents;
        private static JObject bookmarksJSON;
        public static List<Bookmark> bookmarks = new List<Bookmark>();

        public static string chromeDirectory = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\AppData\\Local\\Google\\Chrome\\User Data\\Default\\";
        public string fireQuery = "SELECT moz_bookmarks.title, moz_places.url FROM moz_bookmarks JOIN moz_places ON moz_bookmarks.fk = moz_places.id where parent = 3;";


        public static List<Bookmark> getChromeBookmarks()
        {
            bookmarkLoc = chromeDirectory + "Bookmarks";

            bookmarkContents = File.ReadAllText(bookmarkLoc);

            bookmarksJSON = JObject.Parse(bookmarkContents);

            var bookmarksJ = bookmarksJSON["roots"]["bookmark_bar"]["children"];

            foreach (JObject bookmark in bookmarksJ)
            {
                Bookmark bmark = new Bookmark()
                {
                    name = (string)bookmark["name"],
                    url = (string)bookmark["url"],
                    icon = getChromeFavicon((string)bookmark["url"])
                };

                bookmarks.Add(bmark);
            }

            return bookmarks;
        }

        public static Image getChromeFavicon(string url)
        {
            string dbPath = chromeDirectory + "Favicons";
            var sqlite = new SQLiteConnection($"Data Source={dbPath}");

            SQLiteDataAdapter ad;
            DataTable dt = new DataTable();

            string query = $"SELECT icon_mapping.page_url, favicon_bitmaps.image_data from icon_mapping JOIN favicon_bitmaps ON icon_mapping.icon_id = favicon_bitmaps.icon_id WHERE icon_mapping.page_url = '{url}' ORDER BY favicon_bitmaps.width DESC LIMIT 1";

            try
            {
                SQLiteCommand cmd;
                sqlite.Open();
                cmd = sqlite.CreateCommand();
                cmd.CommandText = query;
                ad = new SQLiteDataAdapter(cmd);
                ad.Fill(dt);
            }
            catch(SQLiteException ex)
            {

            }
            finally
            {
                sqlite.Close();
            }

            Image favicon = null;

            if (dt.Rows.Count == 1)
            {
                MemoryStream ms = new MemoryStream((byte[]) dt.Rows[0]["image_data"]);
                favicon = Image.FromStream(ms);
                ms.Dispose();
                ms.Close();
            }

            return favicon;
        }
    }
}
