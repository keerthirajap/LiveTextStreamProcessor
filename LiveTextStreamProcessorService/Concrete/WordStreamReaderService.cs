namespace LiveTextStreamProcessorService.Concrete
{
    using Booster.CodingTest.Library;
    using LiveTextStreamProcessorService.Interface;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class WordStreamReaderService : IWordStreamReaderService
    {
        private readonly WordStream _wordStream;

        public WordStreamReaderService()
        {
            _wordStream = new WordStream(); // Initialize WordStream
        }

        public Task<int> ReadAsync(byte[] buffer, int offset, int count)
        {
            return _wordStream.ReadAsync(buffer, offset, count);
        }
    }
}