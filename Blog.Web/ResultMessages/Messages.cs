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
    public static class Category
    {
        public static string Add(string categoryName)
        {
            return $"{categoryName} The category with the name has been added successfully";
        }

        public static string Udpate(string categoryName)
        {
            return $"{categoryName} The category with the name been updated successfully";
        }

        public static string Delete(string categoryName)
        {
            return $"{categoryName} The category with the name been deleted successfully";
        }
    }
}