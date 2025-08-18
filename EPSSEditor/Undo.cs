using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPSSEditor
{

    public static class Undo
    {
        private static int currentStep = -1;
        private static Stack<EPSSEditorData> undoableData = null;


        public static EPSSEditorData UndoLastOperation()
        {
            
            if (undoableData.Count > 0)
            {
                currentStep++;
                if (currentStep > undoableData.Count - 1) currentStep = undoableData.Count - 1;
                EPSSEditorData[] array = undoableData.ToArray();
                EPSSEditorData data = array[currentStep];
                return (EPSSEditorData)data.Clone();
            }
            else {
                return null;
            }          
        }


        public static EPSSEditorData RedoLastOperation()
        {

            if (undoableData.Count > 0)
            {
                currentStep--;
                currentStep = Math.Max(0, currentStep);
                EPSSEditorData[] array = undoableData.ToArray();
                EPSSEditorData data = array[currentStep];
                return (EPSSEditorData)data.Clone();
            }
            else
            {
                return null;
            }
        }

        public static void New(EPSSEditorData data)
        {
            if (undoableData != null) undoableData.Clear();
            RegisterUndoChange(data);
        }


        public static void RegisterUndoChange(EPSSEditorData data)
        {
            if (undoableData == null) undoableData = new Stack<EPSSEditorData>();

            // When continuing from redo point, just add next event to the last point.
            undoableData.Push((EPSSEditorData)data.Clone());
            currentStep = 0;
        }
    }
}
