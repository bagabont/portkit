using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PortKit.Crypto
{
    public class DerivationTimeCalibrator
    {
        private readonly TimeSpan _desiredDerivationTime;
        private readonly int _calibrationRounds;
        private readonly int _keySizeInBytes;

        public DerivationTimeCalibrator(TimeSpan desiredDerivationTime, int calibrationRounds, int keySizeInBytes)
        {
            _desiredDerivationTime = desiredDerivationTime;
            _calibrationRounds = calibrationRounds;
            _keySizeInBytes = keySizeInBytes;
        }

        public async Task<int> ComputeCalibratedRoundsAsync(IKeyDerivator derivator, byte[] password, byte[] iv)
        {
            var sw = Stopwatch.StartNew();
            await derivator.DeriveAsync(password, iv, _calibrationRounds, _keySizeInBytes).ConfigureAwait(false);
            sw.Stop();

            var calibratedRounds = _desiredDerivationTime.TotalMilliseconds * _calibrationRounds / sw.ElapsedMilliseconds;

            return Convert.ToInt32(calibratedRounds);
        }
    }
}