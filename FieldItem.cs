namespace FindXMLDifference
{
    public class FieldItem
    {
       

        public FieldItem(string v1, string v2)
        {
            this.FieldName = v1;
            this.FieldType = v2;
        }

        public string FieldType { get; set; }
        public string FieldName { get; set; }

    }
}
