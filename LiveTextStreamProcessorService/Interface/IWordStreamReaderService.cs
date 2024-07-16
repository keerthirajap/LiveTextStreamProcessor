namespace LiveTextStreamProcessorService.Interface
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IWordStreamReaderService
    {
        Task<int> ReadAsync(byte[] buffer, int offset, int count);
    }
}