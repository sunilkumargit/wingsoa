namespace Telerik.Windows.Controls
{
    using System;

    internal class AutomationID
    {
        private string genaratedID = string.Empty;
        private static uint uniqueAutomationID;

        internal string GetID(string templateID)
        {
            if (string.IsNullOrEmpty(this.genaratedID))
            {
                this.genaratedID = templateID + uniqueAutomationID++;
            }
            return this.genaratedID;
        }

        public string Validate(string id, string templateID)
        {
            if (!string.IsNullOrEmpty(id))
            {
                return id;
            }
            return this.GetID(templateID);
        }

        public static string Validate(string id, string templateID, ref AutomationID store)
        {
            if (!string.IsNullOrEmpty(id))
            {
                return id;
            }
            if (store == null)
            {
                store = new AutomationID();
            }
            return store.Validate(string.Empty, templateID);
        }
    }
}

