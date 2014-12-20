﻿// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace MonoGame.Framework.Content.Pipeline.Builder
{
    public class PipelineBuildEvent
    {
        public static readonly string Extension = ".mgcontent";

        public PipelineBuildEvent()
        {
            SourceFile = string.Empty;
            DestFile = string.Empty;
            Importer = string.Empty;
            Processor = string.Empty;
            Parameters = new OpaqueDataDictionary();
            ParametersXml = new List<Pair>();
            Dependencies = new List<string>();
            BuildAsset = new List<string>();
            BuildOutput = new List<string>();
        }

        /// <summary>
        /// Absolute path to the source file.
        /// </summary>
        public string SourceFile { get; set; }

        /// <summary>
        /// The date/time stamp of the source file.
        /// </summary>
        public DateTime SourceTime { get; set; }

        /// <summary>
        /// Absolute path to the output file.
        /// </summary>
        public string DestFile { get; set; }

        /// <summary>
        /// The date/time stamp of the destination file.
        /// </summary>
        public DateTime DestTime { get; set; }

        public string Importer { get; set; }

        public string Processor { get; set; }

        [XmlIgnore]
        public OpaqueDataDictionary Parameters { get; set; }

        public class Pair
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        [XmlElement("Parameters")]
        public List<Pair> ParametersXml { get; set; }

        /// <summary>
        /// Gets or sets the dependencies.
        /// </summary>
        /// <value>The dependencies.</value>
        /// <remarks>
        /// Dependencies are extra files that are required in addition to the <see cref="SourceFile"/>.
        /// Dependencies are added using <see cref="ContentProcessorContext.AddDependency"/>. Changes
        /// to the dependent file causes a rebuilt of the content.
        /// </remarks>
        public List<string> Dependencies { get; set; }

        /// <summary>
        /// Gets or sets the additional (nested) assets.
        /// </summary>
        /// <value>The additional (nested) assets.</value>
        /// <remarks>
        /// <para>
        /// Additional assets are built by using an <see cref="ExternalReference{T}"/> and calling
        /// <see cref="ContentProcessorContext.BuildAndLoadAsset{TInput,TOutput}(ExternalReference{TInput},string)"/>
        /// or <see cref="ContentProcessorContext.BuildAsset{TInput,TOutput}(ExternalReference{TInput},string)"/>.
        /// </para>
        /// <para>
        /// Examples: The mesh processor may build textures and effects in addition to the mesh.
        /// </para>
        /// </remarks>
        public List<string> BuildAsset { get; set; }

        /// <summary>
        /// Gets or sets the related output files.
        /// </summary>
        /// <value>The related output files.</value>
        /// <remarks>
        /// Related output files are non-XNB files that are included in addition to the XNB files.
        /// Related output files need to be copied to the output folder by a content processor and
        /// registered by calling <see cref="ContentProcessorContext.AddOutputFile"/>.
        /// </remarks>
        public List<string> BuildOutput { get; set; }

        public static PipelineBuildEvent Load(string filePath)
        {
            var fullFilePath = Path.GetFullPath(filePath);
            var deserializer = new XmlSerializer(typeof (PipelineBuildEvent));
            PipelineBuildEvent pipelineEvent;
            try
            {
                using (var textReader = new StreamReader(fullFilePath))
                    pipelineEvent = (PipelineBuildEvent) deserializer.Deserialize(textReader);
            }
            catch (Exception)
            {
                return null;
            }

            // Repopulate the parameters from the serialized state.
            foreach (var pair in pipelineEvent.ParametersXml)
                pipelineEvent.Parameters.Add(pair.Key, pair.Value);
            pipelineEvent.ParametersXml.Clear();

            return pipelineEvent;
        }

        public void Save(string filePath)
        {
            var fullFilePath = Path.GetFullPath(filePath);
            // Make sure the directory exists.
            Directory.CreateDirectory(Path.GetDirectoryName(fullFilePath) + Path.DirectorySeparatorChar);

            // Convert the parameters into something we can serialize.
            ParametersXml.Clear();
            foreach (var pair in Parameters)
                ParametersXml.Add(new Pair { Key = pair.Key, Value = ConvertToString(pair.Value) });

            // Serialize our state.
            var serializer = new XmlSerializer(typeof (PipelineBuildEvent));
            using (var textWriter = new StreamWriter(fullFilePath, false, new UTF8Encoding(false)))
                serializer.Serialize(textWriter, this);
        }

        public bool NeedsRebuild(PipelineBuildEvent cachedEvent)
        {
            // If we have no previously cached build event then we cannot
            // be sure that the state hasn't changed... force a rebuild.
            if (cachedEvent == null)
                return true;

            // Verify that the last write time of the source file matches
            // what we recorded when it was built.  If it is different
            // that means someone modified it and we need to rebuild.
            var sourceWriteTime = File.GetLastWriteTime(SourceFile);
            if (cachedEvent.SourceTime != sourceWriteTime)
                return true;

            // Do the same test for the dest file.
            var destWriteTime = File.GetLastWriteTime(DestFile);
            if (cachedEvent.DestTime != destWriteTime)
                return true;

            // If the source file is newer than the dest file
            // then it must have been updated and needs a rebuild.
            if (sourceWriteTime >= destWriteTime)
                return true;

            // Are any of the dependancy files newer than the dest file?
            foreach (var depFile in cachedEvent.Dependencies)
            {
                if (File.GetLastWriteTime(depFile) >= destWriteTime)
                    return true;
            }

            // This shouldn't happen...  but if the source or dest files changed
            // then force a rebuild.
            if (cachedEvent.SourceFile != SourceFile ||
                cachedEvent.DestFile != DestFile)
                return true;

            // Did the importer change?
            // TODO: I need to test the assembly versions here!
            if (cachedEvent.Importer != Importer)
                return true;

            // Did the processor change?
            // TODO: I need to test the assembly versions here!
            if (cachedEvent.Processor != Processor)
                return true;

            // Did the parameters change?
            if (!AreParametersEqual(cachedEvent.Parameters, Parameters))
                return true;

            return false;
        }

        internal static bool AreParametersEqual(OpaqueDataDictionary parameters0, OpaqueDataDictionary parameters1)
        {
            // Same reference or both null?
            if (parameters0 == parameters1)
                return true;

            // Are both dictionaries are empty?
            if ((parameters0 == null || parameters0.Count == 0) && (parameters1 == null || parameters1.Count == 0))
                return true;

            // Is one dictionary empty?
            if (parameters0 == null || parameters1 == null)
                return false;

            // Is number of parameters different?
            // (This assumes that default values are always set the same way, i.e.
            // either parameters with default values are set in both dictionaries
            // or omitted in both dictionaries!)
            if (parameters0.Count != parameters1.Count)
                return false;

            // Compare parameter by parameter.
            foreach (var pair in parameters0)
            {
                object value0 = pair.Value;
                object value1;

                if (!parameters1.TryGetValue(pair.Key, out value1))
                    return false;

                // Are values equal or both null?
                if (Equals(value0, value1))
                    continue;

                // Is one value null?
                if (value0 == null || value1 == null)
                    return false;

                // Values are of different type: Compare string representation.
                if (ConvertToString(value0) != ConvertToString(value1))
                    return false;
            }

            return true;
        }

        private static string ConvertToString(object value)
        {
            if (value == null)
                return null;

            var typeConverter = TypeDescriptor.GetConverter(value.GetType());
            return typeConverter.ConvertToInvariantString(value);
        }
    };
}