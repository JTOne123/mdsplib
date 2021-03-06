using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Numerics;
using mdsplib;
using mdsplib.DSP;
using mdsplib.FT;

namespace UnitTestProject
{
    [TestClass]
    public class DFTTest
    {
        [TestMethod]
        public void Basic()
        {
            // Generate a test signal,
            //      1 Vrms at 20,000 Hz
            //      Sampling Rate = 100,000 Hz
            //      DFT Length is 1000 Points
            double amplitude = 1.0;
            double frequency = 20000;
            UInt32 length = 1000;
            double samplingRate = 100000;
            double[] inputSignal = mdsplib.DSP.Generate.ToneSampling(amplitude, frequency, samplingRate, length);

            // Instantiate a new DFT
            DFT dft = new DFT();

            // Initialize the DFT
            // You only need to do this once or if you change any of the DFT parameters.
            dft.Initialize(length);

            // Call the DFT and get the scaled spectrum back
            Complex[] cSpectrum = dft.Execute(inputSignal);

            // Convert the complex spectrum to magnitude
            double[] lmSpectrum = cSpectrum.Magnitude();

            // Note: At this point lmSpectrum is a 501 byte array that 
            // contains a properly scaled Spectrum from 0 - 50,000 Hz

            // For plotting on an XY Scatter plot generate the X Axis frequency Span
            double[] freqSpan = Util.FFT.FrequencySpan(samplingRate, length);

            // At this point a XY Scatter plot can be generated from,
            // X axis => freqSpan
            // Y axis => lmSpectrum

            // In this example the maximum value of 1 Vrms is located at bin 200 (20,000 Hz)
        }

        [TestMethod]
        public void BasicWnd()
        {
            // Same Input Signal as Example 1
            double amplitude = 1.0; double frequency = 20000;
            UInt32 length = 1000;
            double samplingRate = 100000;
            double[] inputSignal = mdsplib.DSP.Generate.ToneSampling(amplitude, frequency, samplingRate, length);

            // Apply window to the Input Data & calculate Scale Factor
            double[] wCoefs = mdsplib.DSP.Window.Coefficients(mdsplib.DSP.Window.Type.Hamming, length);
            double[] wInputData = inputSignal.Multiply(wCoefs);
            double wScaleFactor = mdsplib.DSP.Window.ScaleFactor.Signal(wCoefs);

            // Instantiate & Initialize a new DFT
            DFT dft = new DFT();
            dft.Initialize(length);

            // Call the DFT and get the scaled spectrum back
            Complex[] cSpectrum = dft.Execute(wInputData);

            // Convert the complex spectrum to note: Magnitude Format
            double[] lmSpectrum = cSpectrum.Magnitude();

            // Properly scale the spectrum for the added window
            lmSpectrum = lmSpectrum.Multiply(wScaleFactor);

            // For plotting on an XY Scatter plot generate the X Axis frequency Span
            double[] freqSpan = Util.FFT.FrequencySpan(samplingRate, length);

            // At this point a XY Scatter plot can be generated from,
            // X axis => freqSpan
            // Y axis => lmSpectrum
        }

        [TestMethod]
        public void BasicWndZeroPad()
        {
            // Same Input Signal as Example 1, except 5 Vrms amplitude
            double amplitude = 5.0; double frequency = 20000;
            UInt32 length = 1000; double samplingRate = 100000;
            double[] inputSignal = mdsplib.DSP.Generate.ToneSampling(amplitude, frequency, samplingRate, length);

            // Apply window to the Input Data & calculate Scale Factor
            double[] wCoefs = mdsplib.DSP.Window.Coefficients(mdsplib.DSP.Window.Type.FTHP, length);
            double[] wInputData = inputSignal.Multiply(wCoefs);
            double wScaleFactor = mdsplib.DSP.Window.ScaleFactor.Signal(wCoefs);

            // Instantiate & Initialize a new DFT w/Zero Padding (5000 points)
            DFT dft = new DFT();
            dft.Initialize(length, 5000);

            // Call the DFT and get the scaled spectrum back
            Complex[] cSpectrum = dft.Execute(wInputData);

            // Convert the complex spectrum to note: Magnitude Format
            double[] lmSpectrum = cSpectrum.Magnitude();

            // Properly scale the spectrum for the added window
            lmSpectrum = lmSpectrum.Multiply(wScaleFactor);

            // For plotting on an XY Scatter plot generate the X Axis frequency Span
            double[] freqSpan = Util.FFT.FrequencySpan(samplingRate, length);

            // At this point a XY Scatter plot can be generated from,
            // X axis => freqSpan
            // Y axis => lmSpectrum

            // For this example - the maximum value of 5 Vrms is at bin 1200 (20,000 Hz)
        }

        [TestMethod]
        public void BasicWndLogMag()
        {
            // Same Input Signal as Example 1, except amplitude is 5 Vrms.
            double amplitude = 5.0; double frequency = 20000;
            UInt32 length = 1000; double samplingRate = 100000;
            double[] inputSignal = mdsplib.DSP.Generate.ToneSampling(amplitude, frequency, samplingRate, length);

            // Apply window to the Input Data & calculate Scale Factor
            double[] wCoefs = mdsplib.DSP.Window.Coefficients(mdsplib.DSP.Window.Type.FTHP, length);
            double[] wInputData = inputSignal.Multiply(wCoefs);
            double wScaleFactor = mdsplib.DSP.Window.ScaleFactor.Signal(wCoefs);

            // Instantiate & Initialize a new DFT
            DFT dft = new DFT();
            dft.Initialize(length);

            // Call the DFT and get the scaled spectrum back
            Complex[] cSpectrum = dft.Execute(wInputData);

            // Convert the complex spectrum to note: Magnitude Format
            double[] lmSpectrum = cSpectrum.Magnitude();

            // Properly scale the spectrum for the added window
            lmSpectrum = lmSpectrum.Multiply(wScaleFactor);

            // Convert from linear magnitude to log magnitude format
            double[] logMagSpectrum = lmSpectrum.MagnitudeDBV();

            // For plotting on an XY Scatter plot generate the X Axis frequency Span
            double[] freqSpan = Util.FFT.FrequencySpan(samplingRate, length);

            // At this point a XY Scatter plot can be generated from,
            // X axis => freqSpan
            // Y axis => logMagSpectrum

            // In this example - maximum amplitude of 13.974 dBV is at bin 200 (20,000 Hz)
        }

        [TestMethod]
        public void NoiseAnalysis()
        {
            // Setup parameters to generate noise test signal of 5 nVrms / rt-Hz
            double amplitude = 5.0e-9;
            UInt32 length = 1000; double samplingRate = 2000;

            // Generate window & calculate Scale Factor for NOISE!
            double[] wCoefs = mdsplib.DSP.Window.Coefficients(mdsplib.DSP.Window.Type.Hamming, length);
            double wScaleFactor = mdsplib.DSP.Window.ScaleFactor.Noise(wCoefs, samplingRate);

            // Instantiate & Initialize a new DFT
            DFT dft = new DFT();
            dft.Initialize(length);

            // Average the noise 'N' times
            Int32 N = 1000;
            double[] noiseSum = new double[(length / 2) + 1];
            for (Int32 i = 0; i < N; i++)
            {
                // Generate the noise signal & apply window
                double[] inputSignal = mdsplib.DSP.Generate.NoisePsd(amplitude, samplingRate, length);
                inputSignal = inputSignal.Multiply(wCoefs);

                // DFT the noise -> Convert -> Sum
                Complex[] cSpectrum = dft.Execute(inputSignal);
                double[] mag2 = cSpectrum.MagnitudeSquared();
                noiseSum = (N == 0) ? noiseSum : noiseSum = noiseSum.Add(mag2);                    
            }

            // Calculate Average, convert to magnitude format
            // See text for the reasons to use Mag^2 format.
            double[] averageNoise = noiseSum.Divide(N);
            double[] lmSpectrum = averageNoise.Magnitude();

            // Properly scale the spectrum for the added window
            lmSpectrum = lmSpectrum.Multiply(wScaleFactor);

            // For plotting on an XY Scatter plot generate the X Axis frequency Span
            double[] freqSpan = Util.FFT.FrequencySpan(samplingRate, length);

            // At this point a XY Scatter plot can be generated from,
            // X axis => freqSpan
            // Y axis => lmSpectrum as a Power Spectral Density Value

            // Extra Credit - Analyze Plot Data Ignoring the first and last 20 Bins
            // Average value should be what we generated = 5.0e-9 Vrms / rt-Hz
            double averageValue = mdsplib.DSP.Analyze.FindMean(lmSpectrum, 20, 20);
        }

        [TestMethod]
        public void PhaseAnalysis()
        {
            // Generate a Phase Ramp between two signals
            double[] resultPhase = new double[360];

            // Instantiate & Initialize a new DFT
            DFT dft = new DFT();
            dft.Initialize(2048);

            for (Int32 phase = 0; phase < 360; phase++)
            {
                double[] inputSignalRef = mdsplib.DSP.Generate.ToneCycles(7.0, 128, 2048);
                double[] inputSignalPhase = mdsplib.DSP.Generate.ToneCycles(7.0, 128, 2048, phaseDeg: phase);

                // Call the DFT and get the scaled spectrum back of a reference and a phase shifted signal.
                Complex[] cSpectrumRef = dft.Execute(inputSignalRef);
                Complex[] cSpectrumPhase = dft.Execute(inputSignalPhase);

                // Magnitude Format - Just as a test point
                //double[] lmSpectrumTest = DSPLib.DSP.ConvertComplex.ToMagnitude(cSpectrumRef);
                //double[] lmSpectrumTestPhase = DSPLib.DSP.ConvertComplex.ToMagnitude(cSpectrumPhase);

                // Extract the phase of bin 128
                double[] resultArrayRef = cSpectrumRef.PhaseDegrees();
                double[] resultArrayPhase = cSpectrumPhase.PhaseDegrees();
                resultPhase[phase] = resultArrayPhase[128] - resultArrayRef[128];
            }

            // resultPhase has a linear phase incrementing signal at this point.
            // Use UnwrapPhase() to remove phase jumps in output data.
        }
    }

    [TestClass]
    public class FFTTest
    {
        [TestMethod]
        public void Basic()
        {
            // Same Input Signal as Example 1, except everything is a power of two
            double amplitude = 1.0; double frequency = 32768;
            UInt32 length = 1024; double samplingRate = 131072;
            double[] inputSignal = mdsplib.DSP.Generate.ToneSampling(amplitude, frequency, samplingRate, length);

            // Apply window to the Input Data & calculate Scale Factor
            double[] wCoefs = mdsplib.DSP.Window.Coefficients(mdsplib.DSP.Window.Type.Hamming, length);
            double[] wInputData = inputSignal.Multiply(wCoefs);
            double wScaleFactor = mdsplib.DSP.Window.ScaleFactor.Signal(wCoefs);

            // Instantiate & Initialize a new DFT
            FFT fft = new FFT();
            fft.Initialize(length);

            // Call the FFT and get the scaled spectrum back
            Complex[] cSpectrum = fft.Direct(wInputData);

            // Convert the complex spectrum to note: Magnitude Squared Format
            // See text for the reasons to use Mag^2 format.
            double[] lmSpectrum = cSpectrum.Magnitude();

            // Properly scale the spectrum for the added window
            lmSpectrum = lmSpectrum.Multiply(wScaleFactor);

            // For plotting on an XY Scatter plot generate the X Axis frequency Span
            double[] freqSpan = Util.FFT.FrequencySpan(samplingRate, length);

            // At this point a XY Scatter plot can be generated from,
            // X axis => freqSpan
            // Y axis => lmSpectrum
        }

        [TestMethod]
        public void BasicWndZeroPad()
        {
            // Same Input Signal as Example 1, except everything is a power of two
            double amplitude = 1.0; double frequency = 32768;
            UInt32 length = 1024; double samplingRate = 131072;
            double[] inputSignal = mdsplib.DSP.Generate.ToneSampling(amplitude, frequency, samplingRate, length);

            // Apply window to the Input Data & calculate Scale Factor
            double[] wCoefs = mdsplib.DSP.Window.Coefficients(mdsplib.DSP.Window.Type.Hamming, length);
            double[] wInputData = inputSignal.Multiply(wCoefs);
            double wScaleFactor = mdsplib.DSP.Window.ScaleFactor.Signal(wCoefs);

            // Instantiate & Initialize a new DFT
            FFT fft = new FFT();
            fft.Initialize(length, length * 3); // Zero Padding = 1024 * 3

            // Call the FFT and get the scaled spectrum back
            Complex[] cSpectrum = fft.Direct(wInputData);

            // Convert the complex spectrum to note: Magnitude Squared Format
            // See text for the reasons to use Mag^2 format.
            double[] lmSpectrum = cSpectrum.Magnitude();

            // Properly scale the spectrum for the added window
            lmSpectrum = lmSpectrum.Multiply(wScaleFactor);

            // For plotting on an XY Scatter plot generate the X Axis frequency Span
            double[] freqSpan = Util.FFT.FrequencySpan(samplingRate, length);

            // At this point a XY Scatter plot can be generated from,
            // X axis => freqSpan
            // Y axis => lmSpectrum
        }

        [TestMethod]
        public void PhaseUnwrap()
        {
            // Generate a Phase Ramp between two signals
            double[] resultPhase = new double[600];
            double[] unwrapPhase = new double[600];

            UInt32 length = 2048;
            double[] wCoeff = mdsplib.DSP.Window.Coefficients(mdsplib.DSP.Window.Type.FTHP, length);

            // Instantiate & Initialize a new DFT
            FFT fft = new FFT();
            fft.Initialize(length, 3 * length);

            for (Int32 phase = 0; phase < 600; phase++)
            {
                double[] inputSignalRef = mdsplib.DSP.Generate.ToneCycles(7.0, 128, length, phaseDeg: 45.0);
                double[] inputSignalPhase = mdsplib.DSP.Generate.ToneCycles(7.0, 128, length, phaseDeg: phase);

                inputSignalRef = inputSignalRef.Multiply(wCoeff);
                inputSignalPhase = inputSignalPhase.Multiply(wCoeff);

                // Call the DFT and get the scaled spectrum back of a reference and a phase shifted signal.
                Complex[] cSpectrumRef = fft.Direct(inputSignalRef);
                Complex[] cSpectrumPhase = fft.Direct(inputSignalPhase);

                // Magnitude Format - Just as a test point
                double[] lmSpectrumTest = cSpectrumRef.Magnitude();
                UInt32 peakLocation = mdsplib.DSP.Analyze.FindMaxPosition(lmSpectrumTest);

                // Extract the phase of 'peak value' bin
                double[] resultArrayRef = cSpectrumRef.PhaseDegrees();
                double[] resultArrayPhase = cSpectrumPhase.PhaseDegrees();
                resultPhase[phase] = resultArrayPhase[peakLocation] - resultArrayRef[peakLocation];
            }
            unwrapPhase = mdsplib.DSP.Analyze.UnwrapPhaseDegrees(resultPhase);
        }

        [TestMethod]
        public void DirectInverse()
        {
            UInt32 length = 2048;

            FFT fft = new FFT();
            fft.Initialize(length);

            double[] wCoeff = mdsplib.DSP.Window.Coefficients(mdsplib.DSP.Window.Type.Hann, length);
            double[] inputSignal = mdsplib.DSP.Generate.ToneSampling(1.0, 4000, 44100, length);
            double[] inputSignalRef = inputSignal.Multiply(inputSignal);

            Complex[] cSpectrum = fft.Direct(inputSignalRef);
            double[] lmSpectrum = cSpectrum.Magnitude();
            Complex[] cSpectrumI = fft.Inverse(cSpectrum);
        }

        [TestMethod]
        public void STFTTest()
        {
            UInt32 fs = 44100;
            double[] wavein = mdsplib.DSP.Generate.Sine(110, fs, 2048);
            var stft = STFT.Direct(wavein);
            double[] reconst = STFT.Inverse(stft, 2048, 0);
            double[] error = wavein.Subtract(reconst);
        }
    }
}
