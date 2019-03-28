using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace ProjectUsingLibraryViewer.Tests.Utilities
{
    [TestFixture]
    public class GitHubUtilityTest
    {
        /// <summary>
        /// https://developer.github.com/v3/ のMockHttpClientをつくって試します。
        /// </summary>
        [Test]
        public async Task TryMockHttpClient()
        {
            var mockHttp = new MockHttpMessageHandler();
            // example:https://api.github.com/repos/rimever/NLP100Knocks/languages
            mockHttp.When("https://api.github.com/repos/*/*/languages")
                .Respond("application/json"
                    , $@"{{
  'なぞのげんご': 9999
}}");
            mockHttp.When("https://api.github.com/licenses").Respond("application/json"
                , $@"{{
       {{
            'key': 'mit',
            'name': 'MIT License',
            'spdx_id': 'MIT',
            'url': 'https://api.github.com/licenses/mit',
            'node_id': 'MDc6TGljZW5zZW1pdA=='
        }}
}}");

            var client = mockHttp.ToHttpClient();

            Console.WriteLine("リポジトリの言語取得サービスのモックを試します。");
            {
                var response =
                    await client.GetAsync("https://api.github.com/repos/user_name/repository_name/languages");

                var json = await response.Content.ReadAsStringAsync();

                Console.WriteLine(json);
            }
            Console.WriteLine("ライセンス一覧取得サービスのモックを試します。");
            {
                var response =
                    await client.GetAsync("https://api.github.com/licenses");

                var json = await response.Content.ReadAsStringAsync();

                Console.WriteLine(json);
            }

        }
    }
}
