using DevExpress.XtraReports.Native;
using Robotics.Base;

namespace Robotics.Web.VTLiveMvC.Support
{
    public class CustomDataSerializer : IDataSerializer
    {
        public const string Name = "customDataSerializer";

        public bool CanDeserialize(string value, string typeName, object extensionProvider)
        {
            return typeName == typeof(functionCall).FullName
                || typeName == typeof(zonesSelector).FullName
                || typeName == typeof(causesSelector).FullName
                || typeName == typeof(incidencesSelector).FullName
                || typeName == typeof(shiftsSelector).FullName
                || typeName == typeof(holidaysSelector).FullName
                || typeName == typeof(terminalsSelector).FullName
                || typeName == typeof(employeesSelector).FullName
                || typeName == typeof(userFieldsSelector).FullName
                || typeName == typeof(userFieldsSelectorRadioBtn).FullName
                || typeName == typeof(taskIdentifier).FullName
                || typeName == typeof(passportIdentifier).FullName
                || typeName == typeof(conceptGroupsSelector).FullName;
        }

        public bool CanSerialize(object data, object extensionProvider)
        {
            return data is functionCall
                || data is zonesSelector
                || data is causesSelector
                || data is incidencesSelector
                || data is shiftsSelector
                || data is holidaysSelector
                || data is terminalsSelector
                || data is employeesSelector
                || data is userFieldsSelector
                || data is userFieldsSelectorRadioBtn
                || data is taskIdentifier
                || data is passportIdentifier
                || data is conceptGroupsSelector;
        }

        public object Deserialize(string value, string typeName, object extensionProvider)
        {
            object result = null;

            switch (typeName)
            {
                case "Robotics.Base.functionCall":
                    result = new functionCall { Value = value };
                    break;

                case "Robotics.Base.zonesSelector":
                    result = new zonesSelector { Value = value };
                    break;

                case "Robotics.Base.causesSelector":
                    result = new causesSelector { Value = value };
                    break;

                case "Robotics.Base.incidencesSelector":
                    result = new incidencesSelector { Value = value };
                    break;

                case "Robotics.Base.shiftsSelector":
                    result = new shiftsSelector { Value = value };
                    break;

                case "Robotics.Base.holidaysSelector":
                    result = new holidaysSelector { Value = value };
                    break;

                case "Robotics.Base.terminalsSelector":
                    result = new terminalsSelector { Value = value };
                    break;

                case "Robotics.Base.employeesSelector":
                    result = new employeesSelector { Value = value };
                    break;

                case "Robotics.Base.userFieldsSelector":
                    result = new userFieldsSelector { Value = value };
                    break;
                case "Robotics.Base.userFieldsSelectorRadioBtn":
                    result = new userFieldsSelectorRadioBtn { Value = value };
                    break;
                case "Robotics.Base.taskIdentifier":
                    result = new taskIdentifier { Value = value };
                    break;

                case "Robotics.Base.passportIdentifier": // Identificador de usuario
                    result = new passportIdentifier { Value = value };
                    break;

                case "Robotics.Base.conceptIdentifier":     // Identificador de saldo
                    result = new conceptIdentifier { Value = value };
                    break;

                case "Robotics.Base.causeIdentifier":     // Identificador de justificación
                    result = new causeIdentifier { Value = value };
                    break;

                case "Robotics.Base.incidenceIdentifier":     // Identificador de incidencia
                    result = new incidenceIdentifier { Value = value };
                    break;

                case "Robotics.Base.userFieldIdentifier":   // Identificador de ficha
                    result = new userFieldIdentifier { Value = value };
                    break;

                case "Robotics.Base.conceptsSelector":  // SALDOS ------------------------------
                    result = new conceptsSelector { Value = value };
                    break;

                case "Robotics.Base.conceptGroupsSelector":
                    result = new conceptGroupsSelector { Value = value };
                    break;
            }

            return result;
        }

        public string Serialize(object data, object extensionProvider)
        {
            object result = null;

            switch (data.GetType().FullName)
            {
                case "Robotics.Base.functionCall":
                    result = data as functionCall;
                    break;

                case "Robotics.Base.zonesSelector":
                    result = data as zonesSelector;
                    break;

                case "Robotics.Base.causesSelector":
                    result = data as causesSelector;
                    break;

                case "Robotics.Base.incidencesSelector":
                    result = data as incidencesSelector;
                    break;

                case "Robotics.Base.shiftsSelector":
                    result = data as shiftsSelector;
                    break;
                case "Robotics.Base.holidaysSelector":
                    result = data as holidaysSelector;
                    break;

                case "Robotics.Base.terminalsSelector":
                    result = data as terminalsSelector;
                    break;

                case "Robotics.Base.employeesSelector":
                    result = data as employeesSelector;
                    break;

                case "Robotics.Base.userFieldsSelector":
                    result = data as userFieldsSelector;
                    break;
                case "Robotics.Base.userFieldsSelectorRadioBtn":
                    result = data as userFieldsSelectorRadioBtn;
                    break;

                case "Robotics.Base.taskIdentifier":
                    result = data as taskIdentifier;
                    break;

                case "Robotics.Base.passportIdentifier": // Identificador de usuario
                    result = data as passportIdentifier;
                    break;

                case "Robotics.Base.conceptIdentifier":     // Identificador de saldo
                    result = data as conceptIdentifier;
                    break;

                case "Robotics.Base.causeIdentifier":     // Identificador de justificación
                    result = data as causeIdentifier;
                    break;

                case "Robotics.Base.incidenceIdentifier":     // Identificador de incidencia
                    result = data as incidenceIdentifier;
                    break;

                case "Robotics.Base.userFieldIdentifier":   // Identificador de ficha
                    result = data as userFieldIdentifier;
                    break;

                case "Robotics.Base.conceptGroupsSelector":
                    result = data as conceptGroupsSelector;
                    break;

                case "Robotics.Base.conceptsSelector":
                    result = data as conceptsSelector;
                    break;
            }

            return result != null ? result.ToString() : null;
        }
    }
}