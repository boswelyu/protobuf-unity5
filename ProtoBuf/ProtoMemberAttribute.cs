﻿using System;

namespace ProtoBuf
{
    /// <summary>
    /// Declares a member to be used in protocol-buffer serialization, using
    /// the given Tag. A DataFormat may be used to optimise the serialization
    /// format (for instance, using zigzag encoding for negative numbers, or 
    /// fixed-length encoding for large values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field,
        AllowMultiple = false, Inherited = true)]
    public class ProtoMemberAttribute : Attribute
    {
        /// <summary>
        /// Creates a new ProtoMemberAttribute instance.
        /// </summary>
        /// <param name="tag">Specifies the unique tag used to identify this member within the type.</param>
        public ProtoMemberAttribute(int tag)
        {
            if (tag <= 0) throw new ArgumentOutOfRangeException("tag");
            this.tag = tag;
        }
        
        /// <summary>
        /// Gets or sets the original name defined in the .proto; not used
        /// during serialization.
        /// </summary>
        public string Name { get { return name; } set { name = value; } }
        private string name;

        /// <summary>
        /// Gets or sets the data-format to be used when encoding this value.
        /// </summary>
        public DataFormat DataFormat { get { return dataFormat; } set { dataFormat = value; } }
        private DataFormat dataFormat; 

        /// <summary>
        /// Gets the unique tag used to identify this member within the type.
        /// </summary>
        public int Tag { get { return tag; } }
        private readonly int tag;

        /// <summary>
        /// Gets or sets a value indicating whether this member is mandatory.
        /// </summary>
        public bool IsRequired {
            get { return (options & MemberSerializationOptions.Required) == MemberSerializationOptions.Required; }
            set {
                if (value) options |= MemberSerializationOptions.Required;
                else options &= ~MemberSerializationOptions.Required;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this member is packed (lists/arrays).
        /// </summary>
        public MemberSerializationOptions Options { get { return options; } set { options = value; } }
        private MemberSerializationOptions options;

        
    }

    /// <summary>
    /// Additional (optional) settings that control serialization of members
    /// </summary>
    [Flags]
    public enum MemberSerializationOptions
    {
        /// <summary>
        /// Default; no additional options
        /// </summary>
        None = 0,
        /// <summary>
        /// Indicates that repeated elements should use packed (length-prefixed) encoding
        /// </summary>
        Packed = 1,
        /// <summary>
        /// Indicates that the given item is required
        /// </summary>
        Required = 2
    }

    /// <summary>
    /// Declares a member to be used in protocol-buffer serialization, using
    /// the given Tag and MemberName. This allows ProtoMemberAttribute usage
    /// even for partial classes where the individual members are not
    /// under direct control.
    /// A DataFormat may be used to optimise the serialization
    /// format (for instance, using zigzag encoding for negative numbers, or 
    /// fixed-length encoding for large values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class,
            AllowMultiple = true, Inherited = true)]
    public class ProtoPartialMemberAttribute : ProtoMemberAttribute
    {
        /// <summary>
        /// Creates a new ProtoMemberAttribute instance.
        /// </summary>
        /// <param name="tag">Specifies the unique tag used to identify this member within the type.</param>
        /// <param name="memberName">Specifies the member to be serialized.</param>
        public ProtoPartialMemberAttribute(int tag, string memberName)
            : base(tag)
        {
            if (string.IsNullOrEmpty(memberName)) throw new ArgumentNullException("memberName");
            this.memberName = memberName;
        }
        /// <summary>
        /// The name of the member to be serialized.
        /// </summary>
        public string MemberName { get { return memberName; } }
        private readonly string memberName;
    }
}
