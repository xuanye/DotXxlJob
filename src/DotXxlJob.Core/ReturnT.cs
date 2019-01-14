namespace DotXxlJob.Core
{
    public class ReturnT<T> 
    {
        public  int Code { get; set; }
        
        public string Msg { get; set; }
        
        public  T Content { get; set; }
    }
}