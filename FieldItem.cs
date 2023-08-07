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

        public override string ToString()
        {
            return this.FieldName;
        }

        public override bool Equals(object obj)
        {
            if (obj is FieldItem)
            {
                var item = obj as FieldItem;
                return this.FieldName == item.FieldName && this.FieldType == item.FieldType;
            }
            return false;
        }
    }
}
