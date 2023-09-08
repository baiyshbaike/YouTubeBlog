namespace Blog.Web.ResultMessages;

public static class Messages
{
    public static class Article
    {
        public static string Add(string articleTitle)
        {
            return $"{articleTitle} The article with the title has been added successfully";
        }
    
        public static string Udpate(string articleTitle)
        {
            return $"{articleTitle} The article with the title has been updated successfully";
        }

        public static string Delete(string articleTitle)
        {
            return $"{articleTitle} The article with the title has been deleted successfully";
        }
    }
}