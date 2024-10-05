using System;
using System.Diagnostics;
using System.IO;
using Xiph.Easy.OggVorbis;
using Xiph.Ogg;
using Xiph.Vorbis;

namespace VorbisInterop
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var pcmBytes = File.ReadAllBytes(@"C:\Users\cab45\Desktop\hello.raw");

            float[,] data = ConvertRawPCMFile(44100, 2, pcmBytes, 44100, 2);

            //var sw = Stopwatch.StartNew();
            //var setup = new OggVorbisEncoderSetup()
            //{
            //    Channels = 2,
            //    SampleRate = 44100,
            //    BitRate = VorbisBitrates.VariableBitrate(0.4f),
            //};

            using var memoryStream = new MemoryStream();
            //using var outputstream = new FileStream(@"C:\Users\cab45\Desktop\encoded.ogg", FileMode.Create);
            //using (var state = setup.StartEncode(outputstream))
            //{
            //    state.Write(data);
            //}
            //sw.Stop();
            //Console.WriteLine($"ms: {sw.ElapsedMilliseconds}");

            var info = new VorbisInfo();
            info.EncodeInitVbr(2, 44100, -0.6f);
            var comment = new VorbisComment();
            comment.Add("my test comment");
            var dspState = info.AnalysisInit();
            var block = new VorbisBlock(dspState);
            var oggStreamState = new OggStreamState();
            dspState.WriteHeaders(oggStreamState, comment);
            oggStreamState.FlushAll(memoryStream);
            dspState.Write(data);
            dspState.Encode(oggStreamState, block, memoryStream);
            oggStreamState.Dispose();
            block.Dispose();
            dspState.Dispose();
            comment.Dispose();
            info.Dispose();

            File.WriteAllBytes(@"C:\Users\cab45\Desktop\encoded.ogg", memoryStream.ToArray());

            Console.ReadLine();
        }

        private static float[,] ConvertRawPCMFile(int OutputSampleRate, int OutputChannels, byte[] PCMSamples, int PCMSampleRate, int PCMChannels)
        {
            int NumPCMSamples = (PCMSamples.Length / 2 / PCMChannels);
            float PCMDuraton = NumPCMSamples / (float)PCMSampleRate;

            int NumOutputSamples = (int)(PCMDuraton * OutputSampleRate);

            float[,] OutSamples = new float[OutputChannels, NumOutputSamples];

            for (int sampleNumber = 0; sampleNumber < NumOutputSamples; sampleNumber++)
            {
                for (int ch = 0; ch < OutputChannels; ch++)
                {
                    int sampleIndex = (sampleNumber * PCMChannels) * 2;

                    if (ch < PCMChannels) sampleIndex += (ch * 2);

                    float rawSample = ShortToSample((short)(PCMSamples[sampleIndex + 1] << 8 | PCMSamples[sampleIndex]));
                    OutSamples[ch, sampleNumber] = rawSample;
                }
            }

            return OutSamples;
        }

        private static float ShortToSample(short pcmValue)
        {
            return pcmValue / 32768f;
        }

        private static float ByteToSample(short pcmValue)
        {
            return pcmValue / 128f;
        }
    }
}
