using System.Text.Json.Serialization;

namespace KintaiSystem.Infrastructure.Json
{
    /// <summary>
    /// Microsoft Graph APIを使用して取得したユーザー情報のJSON
    /// </summary>
    public class GraphUserJsonData
    {
        /*
         * 限定的なプロパティのセット
         * businessPhones、
         * displayName、
         * givenName、
         * id
         * jobTitle、
         * mail、
         * mobilePhone、
         * officeLocation、
         * preferredLanguage、
         * surname
         * userPrincipalName
         * 
         * all properties:
         * https://docs.microsoft.com/ja-jp/graph/api/resources/user?view=graph-rest-1.0#properties
         */


        /// <summary>姓</summary>
        [JsonPropertyName("surname")]
        public string LastName { get; set; }

        /// <summary>名</summary>
        [JsonPropertyName("givenName")]
        public string FirstName { get; set; }

        /// <summary>メールアドレス</summary>
        [JsonPropertyName("mail")]
        public string MailAddress { get; set; }

        /// <summary>従業員ID</summary>
        [JsonPropertyName("employeeId")]
        public string EmployeeId { get; set; }
    }
}
