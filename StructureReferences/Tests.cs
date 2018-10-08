using FluentAssertions;
using System.Runtime.InteropServices;
using Xunit;

namespace StructureReferences
{
	public sealed class Tests
	{
		Structure _local;

		readonly Structure _structure;

		public Tests() : this(new Structure(0)) {}

		Tests(Structure structure) => _structure = structure;

		[Fact]
		public void Assignments()
		{
			var one   = Add(_structure);
			var two   = Add(one);
			var three = Add(two);
			three.Count.Should().Be(3);
		}

		[Fact]
		public void Inline() // Doesn't pass.
		{
			Add(in Add(in Add(in _structure))).Count.Should().Be(3);
		}

		[Fact]
		public void Local()
		{
			var one   = AddFromLocal(_structure);
			var two   = AddFromLocal(one);
			var three = AddFromLocal(two);
			three.Count.Should().Be(3);
		}

		[Fact]
		public void InlineLocal()
		{
			AddFromLocal(AddFromLocal(AddFromLocal(_structure))).Count.Should().Be(3);
		}

		static ref readonly Structure Add(in Structure parameter)
		{
			var structure = new Structure(parameter.Count + 1);
			var span      = MemoryMarshal.CreateReadOnlySpan(ref structure, 0);
			return ref MemoryMarshal.GetReference(span);
		}

		ref readonly Structure AddFromLocal(in Structure parameter)
		{
			_local = new Structure(parameter.Count + 1);
			return ref _local;
		}
	}

	public readonly struct Structure
	{
		public Structure(uint count) => Count = count;

		public uint Count { get; }
	}
}