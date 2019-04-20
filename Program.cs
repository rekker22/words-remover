using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Drawing;

namespace CSHttpClientSample
{
    static class Program
    {
        // Replace <Subscription Key> with your valid subscription key.
        const string subscriptionKey = "cf9de33d461940c1a5c69cc71b5d0984";

        // You must use the same Azure region in your REST API method as you used to
        // get your subscription keys. For example, if you got your subscription keys
        // from the West US region, replace "westcentralus" in the URL
        // below with "westus".
        //
        // Free trial subscription keys are generated in the "westus" region.
        // If you use a free trial subscription key, you shouldn't need to change
        // this region.
        const string uriBase =
            "https://westcentralus.api.cognitive.microsoft.com/vision/v2.0/ocr";

        static void Main()
        {
            // Get the path and filename to process from the user.
            Console.WriteLine("Optical Character Recognition:");
            //Console.Write("Enter the path to an image with text you wish to read: ");
            string imageFilePath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + @"\data\uc_003.JPEG";

            if (File.Exists(imageFilePath))
            {
                // Call the REST API method.
                Console.WriteLine("\nWait a moment for the results to appear.\n");
                MakeOCRRequest(imageFilePath).Wait();
            }
            else
            {
                Console.WriteLine("File path is||" + imageFilePath + "||Which is Wrong");
            }
            Console.WriteLine("\nPress Enter to exit...");
            Console.ReadLine();
        }

        /// <summary>
        /// Gets the text visible in the specified image file by using
        /// the Computer Vision REST API.
        /// </summary>
        /// <param name="imageFilePath">The image file with printed text.</param>
        static async Task MakeOCRRequest(string imageFilePath)
        {

            try
            {
                var contentString = "";

                HttpClient client = new HttpClient();

                // Request headers.
                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key", subscriptionKey);

                // Request parameters. 
                // The language parameter doesn't specify a language, so the 
                // method detects it automatically.
                // The detectOrientation parameter is set to true, so the method detects and
                // and corrects text orientation before detecting text.
                string requestParameters = "language=ja&detectOrientation=false";

                // Assemble the URI for the REST API method.
                string uri = uriBase + "?" + requestParameters;

                HttpResponseMessage response;

                bool empty = false;//Needs for returning data

                //**************Looping starts here until we get all the words using do while
                do
                {
                    // Read the contents of the specified local image
                    // into a byte array.
                    byte[] byteData = GetImageAsByteArray(imageFilePath);

                    // Add the byte array as an octet stream to the request body.
                    using (ByteArrayContent content = new ByteArrayContent(byteData))
                    {
                        // This example uses the "application/octet-stream" content type.
                        // The other content types you can use are "application/json"
                        // and "multipart/form-data".
                        content.Headers.ContentType =
                            new MediaTypeHeaderValue("application/octet-stream");

                        // Asynchronously call the REST API method.
                        response = await client.PostAsync(uri, content);
                    }

                    // Asynchronously get the JSON response.
                    contentString = await response.Content.ReadAsStringAsync();

                    contentString.GetType();
                    dynamic str = JsonConvert.DeserializeObject(contentString);   //Instead of content string give the json file dynamic
                                                                                  // str = JsonConvert.DeserializeObject(File.ReadAllText(MyFilePath));

                    var v = str.regions.Count; //Checking is the regions array is empty or not

                    if (v == 0)
                        empty = true;
                   
                    Bitmap b = new Bitmap(imageFilePath);

                    foreach (var region in str.regions)
                    {
                        foreach (var line in region.lines)//main part of parsing
                        {
                            foreach (var words in line.words)
                            {
                                Console.WriteLine(words.boundingBox + "\n");

                                string bb = words.boundingBox;

                                string[] a = bb.Split(',');

                                int[] arr = Array.ConvertAll(a, int.Parse);

                                micro_post.drw d = new micro_post.drw();

                                b = d.DrawLineInt(b, arr[0], arr[1], arr[2], arr[3]);
                            }
                        }
                    }
                    b.Save(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + @"\data\uc_004.JPEG");
                    b.Dispose();
                    if(File.Exists(imageFilePath) )
                        File.Delete(imageFilePath);
                    File.Move(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + @"\data\uc_004.JPEG", imageFilePath);
                    

                } while(empty == false);

                //b.Save("Edited", System.Drawing.Imaging.ImageFormat.Jpeg);

                //get the size of Jtoken;

                //Showing in unicode
                //Console.OutputEncoding = System.Text.Encoding.UTF8;
                
                // Display the JSON response.
                //Console.WriteLine("\nResponse:\n\n{0}\n",
                //    JToken.Parse(contentString).ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message);
            }
        }

        /// <summary>
        /// Returns the contents of the specified file as a byte array.
        /// </summary>
        /// <param name="imageFilePath">The image file to read.</param>
        /// <returns>The byte array of the image data.</returns>
        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            // Open a read-only file stream for the specified file.
            using (FileStream fileStream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                // Read the file's contents into a byte array.
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }
    }
}