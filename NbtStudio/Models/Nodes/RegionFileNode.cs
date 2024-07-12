﻿using fNbt;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Forms;

namespace NbtStudio
{
    public class RegionFileNode : ModelNode<Chunk>
    {
        public readonly RegionFile Region;
        public RegionFileNode(NbtTreeModel tree, INode parent, RegionFile file) : base(tree, parent)
        {
            Region = file;
            Region.ChunksChanged += Region_ChunksChanged;
            Region.ActionPerformed += Region_ActionPerformed;
            Region.OnSaved += Region_OnSaved;
        }

        protected override void SelfDispose()
        {
            Region.ChunksChanged -= Region_ChunksChanged;
            Region.ActionPerformed -= Region_ActionPerformed;
            Region.OnSaved -= Region_OnSaved;
        }

        private void Region_OnSaved()
        {
            RefreshChildren();
        }

        private void Region_ActionPerformed(UndoableAction action)
        {
            NoticeAction(action);
        }

        private void Region_ChunksChanged()
        {
            RefreshChildren();
        }

        protected override IEnumerable<Chunk> GetChildren()
        {
            return Region.AllChunks;
        }

        public override string Description => Region.Path is null ? "unsaved region file" : System.IO.Path.GetFileName(Region.Path);

        public override bool CanCopy => Region.Path is not null;
        public override DataObject Copy()
        {
            var data = new DataObject();
            if (Region.Path is not null)
                data.SetFileDropList(new StringCollection { Region.Path });
            return data;
        }
        public override bool CanCut => Region.Path is not null;
        public override DataObject Cut() => FileNodeOperations.Cut(Region.Path);
        public override bool CanDelete => true;
        public override void Delete()
        {
            FileNodeOperations.DeleteFile(Region.Path);
            base.Delete();
        }
        public override bool CanEdit => Region.Path is not null;
        public override bool CanPaste => true;
        public override bool CanRename => Region.Path is not null;
        public override bool CanSort => false;
        public override IEnumerable<INode> Paste(IDataObject data)
        {
            var tags = NbtNodeOperations.ParseTags(data).OfType<NbtCompound>().ToList();
            var available = Region.GetAvailableCoords();
            var chunks = Enumerable.Zip(available, tags, (slot, tag) => Chunk.EmptyChunk(tag, slot.x, slot.z)).ToList();
            foreach (var chunk in chunks)
            {
                Region.AddChunk(chunk);
            }
            return NodeChildren(chunks);
        }
        public override bool CanReceiveDrop(IEnumerable<INode> nodes) => nodes.All(x => x is ChunkNode);
        public override void ReceiveDrop(IEnumerable<INode> nodes, int index)
        {
            var chunks = nodes.Filter(x => x.Get<Chunk>()).ToList();
            foreach (var chunk in chunks)
            {
                if (Region.GetChunk(chunk.X, chunk.Z) is null)
                    Region.AddChunk(chunk);
            }
        }
    }
}
