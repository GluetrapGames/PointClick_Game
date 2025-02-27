//	Copyright (c) 2012 Calvin Rien
//        http://the.darktable.com
//
//	This software is provided 'as-is', without any express or implied warranty. In
//	no event will the authors be held liable for any damages arising from the use
//	of this software.
//
//	Permission is granted to anyone to use this software for any purpose,
//	including commercial applications, and to alter it and redistribute it freely,
//	subject to the following restrictions:
//
//	1. The origin of this software must not be misrepresented; you must not claim
//	that you wrote the original software. If you use this software in a product,
//	an acknowledgment in the product documentation would be appreciated but is not
//	required.
//
//	2. Altered source versions must be plainly marked as such, and must not be
//	misrepresented as being the original software.
//
//	3. This notice may not be removed or altered from any source distribution.
//
//  =============================================================================
//
//  derived from Gregorio Zanon's script
//  http://forum.unity3d.com/threads/119295-Writing-AudioListener.GetOutputData-to-wav-problem?p=806734&viewfull=1#post806734


// Modified by Piotr Piwoni

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class SavWav
{
	private const int HEADER_SIZE = 44;

	public static bool Save(string filePath, AudioClip clip)
	{
		if (!filePath.ToLower().EndsWith(".wav"))
			filePath += ".wav";

		try
		{
			// Ensure the directory exists
			Directory.CreateDirectory(Path.GetDirectoryName(filePath));

			using (FileStream fileStream = CreateEmpty(filePath))
			{
				ConvertAndWrite(fileStream, clip);
				WriteHeader(fileStream, clip);
			}

			return true;
		}
		catch (Exception e)
		{
			Debug.LogError($"Failed to save WAV file: {e.Message}");
			return false;
		}
	}

	public static AudioClip TrimSilence(AudioClip clip, float min)
	{
		var samples = new float[clip.samples];
		clip.GetData(samples, 0);
		return TrimSilence(new List<float>(samples), min, clip.channels,
			clip.frequency);
	}

	public static AudioClip TrimSilence(List<float> samples, float min,
		int channels, int hz)
	{
		return TrimSilence(samples, min, channels, hz, false, false);
	}

	public static AudioClip TrimSilence(List<float> samples, float min,
		int channels, int hz, bool _3D, bool stream)
	{
		var trimmedSamples =
			new List<float>(
				samples); // Clone list to avoid modifying the original

		var start = trimmedSamples.FindIndex(s => Mathf.Abs(s) > min);
		var end = trimmedSamples.FindLastIndex(s => Mathf.Abs(s) > min);

		if (start < 0 || end < 0) return null;

		trimmedSamples = trimmedSamples.GetRange(start, end - start + 1);

		var clip = AudioClip.Create("TrimmedClip", trimmedSamples.Count,
			channels, hz, _3D, stream);
		clip.SetData(trimmedSamples.ToArray(), 0);

		return clip;
	}

	private static FileStream CreateEmpty(string filePath)
	{
		var fileStream = new FileStream(filePath, FileMode.Create);
		var emptyHeader = new byte[HEADER_SIZE];
		fileStream.Write(emptyHeader, 0, HEADER_SIZE);
		return fileStream;
	}

	private static void ConvertAndWrite(FileStream fileStream, AudioClip clip)
	{
		var samples = new float[clip.samples];
		clip.GetData(samples, 0);

		var intData = new short[samples.Length];
		var bytesData = new byte[samples.Length * 2];

		const int rescaleFactor = 32767;
		var buffer = new byte[2];

		for (var i = 0; i < samples.Length; i++)
		{
			intData[i] = (short)(samples[i] * rescaleFactor);
			BitConverter.GetBytes(intData[i]).CopyTo(bytesData, i * 2);
		}

		fileStream.Write(bytesData, 0, bytesData.Length);
	}

	private static void WriteHeader(FileStream fileStream, AudioClip clip)
	{
		var hz = clip.frequency;
		var channels = clip.channels;
		var samples = clip.samples;

		fileStream.Seek(0, SeekOrigin.Begin);

		fileStream.Write(Encoding.UTF8.GetBytes("RIFF"), 0, 4);
		fileStream.Write(BitConverter.GetBytes(fileStream.Length - 8), 0, 4);
		fileStream.Write(Encoding.UTF8.GetBytes("WAVE"), 0, 4);
		fileStream.Write(Encoding.UTF8.GetBytes("fmt "), 0, 4);
		fileStream.Write(BitConverter.GetBytes(16), 0, 4);
		fileStream.Write(BitConverter.GetBytes((ushort)1), 0, 2);
		fileStream.Write(BitConverter.GetBytes((ushort)channels), 0, 2);
		fileStream.Write(BitConverter.GetBytes(hz), 0, 4);
		fileStream.Write(BitConverter.GetBytes(hz * channels * 2), 0, 4);
		fileStream.Write(BitConverter.GetBytes((ushort)(channels * 2)), 0, 2);
		fileStream.Write(BitConverter.GetBytes((ushort)16), 0, 2);
		fileStream.Write(Encoding.UTF8.GetBytes("data"), 0, 4);
		fileStream.Write(BitConverter.GetBytes(samples * channels * 2), 0, 4);
	}
}