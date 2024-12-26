using TikaOnDotNet.TextExtraction;
using DocumentFormat.OpenXml.Packaging;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Embeddings;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace ChatBot.Server.Helpers
{
    public class ModelHelper : IModelHelper
    {
        //public async Task<string> ProcessFileAsync(string filePath)
        //{
        //    // Example: Extract text from the file
        //    string extractedText = await ExtractTextFromFileAsync(filePath);

        //    // Example: Process text with RAG model
        //    string result = await QueryRagModelAsync(extractedText);

        //    return result;
        //}

        //private async Task<string> ExtractTextFromFileAsync(string filePath)
        //{
        //    // Use a library like TikaOnDotNet, iText7, or PdfSharp to extract text
        //    // This is a placeholder for actual extraction logic
        //    return await File.ReadAllTextAsync(filePath);
        //}

        //public string ExtractTextFromFile(string filePath)
        //{
        //    var textExtractor = new TextExtractor();
        //    var result = textExtractor.Extract(filePath);
        //    return result.Text;
        //}
        //public string ExtractTextFromWord(string filePath)
        //{
        //    using var wordDocument = WordprocessingDocument.Open(filePath, false);
        //    var body = wordDocument.MainDocumentPart.Document.Body;
        //    return body.InnerText;
        //}

        //private async Task<string> QueryRagModelAsync(string text)
        //{
        //    // Placeholder for calling the RAG model API
        //    // Send text to a model like OpenAI, Azure OpenAI, or Hugging Face
        //    return $"Processed text: {text}";
        //}


        private readonly string _openAiApiKey;
        private readonly string _vectorDatabaseUrl;

        public ModelHelper(IConfiguration configuration)
        {
            _openAiApiKey = configuration["OpenAI:ApiKey"];
            _vectorDatabaseUrl = configuration["VectorDatabase:Url"]; // Example: URL for Pinecone or FAISS
        }

        // 1. Extract text from PDF
        public async Task<string> ExtractTextFromPdfAsync(string filePath)
        {
            StringBuilder text = new StringBuilder();
            using (PdfDocument document = PdfReader.Open(filePath, PdfDocumentOpenMode.ReadOnly))
            {
                foreach (var page in document.Pages)
                {
                    text.AppendLine(page.Contents.ToString());
                }
            }
            return text.ToString();
        }

        // 2. Extract text from DOCX (you can extend this for other formats)
        public string ExtractTextFromDocx(string filePath)
        {
            // Use a library like Open XML SDK or NPOI to extract text from DOCX
            // For simplicity, assume a method that gets the text content of DOCX.
            return File.ReadAllText(filePath); // Placeholder for actual DOCX extraction logic
        }

        // 3. Generate vector embeddings for the extracted text
        public async Task<List<float>> GenerateEmbeddingsAsync(string text)
        {
            var openAi = new OpenAIClient(_openAiApiKey);
          
            EmbeddingClient client = new("text-embedding-3-small", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
            var result = await openAi.
                .CreateEmbeddingAsync(text, "text-embedding-ada-002");
           
            // Assuming the result returns a list of floats as embeddings
            return result.Data.First().Embedding;
        }

        // 4. Store the embeddings in a vector database (example: Pinecone, FAISS, or simple in-memory DB)
        public async Task StoreEmbeddingsAsync(string documentId, List<float> embeddings)
        {
            // Example storing in a vector database (Pinecone, FAISS, etc.)
            // For simplicity, we'll use an in-memory database

            var vectorDatabase = new Dictionary<string, List<float>>(); // Simulating a vector DB

            // Store the embeddings by document ID
            vectorDatabase[documentId] = embeddings;

            // You would replace this with an actual call to a vector database service
            await Task.CompletedTask; // Simulate async DB operation
        }

        // 5. Main method to process uploaded document (PDF, DOCX, etc.)
        public async Task ProcessDocumentAsync(string filePath, string documentId)
        {
            string text = string.Empty;

            // Extract text based on file extension
            if (filePath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                text = await ExtractTextFromPdfAsync(filePath);
            }
            else if (filePath.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
            {
                text = ExtractTextFromDocx(filePath);
            }
            else
            {
                throw new InvalidOperationException("Unsupported file format.");
            }

            // Generate embeddings from the extracted text
            var embeddings = await GenerateEmbeddingsAsync(text);

            // Store the embeddings in the vector database
            await StoreEmbeddingsAsync(documentId, embeddings);
        }

        // 6. Helper method to clean up the text (optional, e.g., remove special characters)
        private string CleanText(string text)
        {
            // Remove special characters, excessive spaces, etc.
            return Regex.Replace(text, @"\s+", " ").Trim();
        }
    }
}
