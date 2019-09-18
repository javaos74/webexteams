using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities.Presentation.Metadata;
using System.ComponentModel;


namespace WebexTeams
{
    public class DesignerMetadata : IRegisterMetadata
    {
        public void Register()
        {
            AttributeTableBuilder attributeTableBuilder = new AttributeTableBuilder();
            attributeTableBuilder.AddCustomAttributes(typeof(ListRooms), new DesignerAttribute(typeof(ListRoomsDesigner)));
            attributeTableBuilder.AddCustomAttributes(typeof(SendMessage), new DesignerAttribute(typeof(SendMessageDesigner)));
            MetadataStore.AddAttributeTable(attributeTableBuilder.CreateTable());
        }
    }
}
