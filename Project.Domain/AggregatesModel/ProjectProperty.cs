using Project.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Domain.AggregatesModel
{

    /// <summary>
    /// 项目属性
    /// </summary>
    public class ProjectProperty : ValueObject
    {

        public int ProjectId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public string Text { get; set; }
        public ProjectProperty() { }

        public ProjectProperty(string key, string value, string text)
        {
            Key = key;
            Value = value;
            Text = text;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Key;
            yield return Value;
            yield return Text;
        }


    }
}
