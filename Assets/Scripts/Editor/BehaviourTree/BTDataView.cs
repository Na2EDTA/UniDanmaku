using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;


public class BTDataView : Node
{
    public BTData data;
    public Port input, output;
    public Action<BTDataView> OnDataSelected;

    public BTDataView(BTData data) : base("Assets/Scripts/Editor/BehaviourTree/BTDataView.uxml")
    {
        this.data = data;
        title = data.name;
        viewDataKey = data.guid;

        style.left = data.position.x;
        style.top = data.position.y;

        CreateOutputPorts();
        CreateInputPorts();
    }

    private void CreateInputPorts()
    {
        input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(float));
        input.portColor = new(0.5f, 0.75f, 0.5f, 1);
        if (input != null)
        {
            input.style.flexDirection = FlexDirection.Row;
            input.portName = " ";
            inputContainer.Add(input);
        }
    }

    private void CreateOutputPorts()
    {
        output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
        output.portColor = new(0.5f, 0.75f, 0.5f, 1);
        if (output != null)
        {
            output.style.flexDirection = FlexDirection.RowReverse;
            output.portName = " ";
            outputContainer.Add(output);
        }
    }

    //֧������϶�λ�ã����ҿ��Գ���
    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Undo.RecordObject(data, "Behaviour Tree (Set position)");
        data.position.x = newPos.xMin;
        data.position.y = newPos.yMin;
        EditorUtility.SetDirty(data);
    }

    //ѡ��ʱ��Ϊ
    public override void OnSelected()
    {
        base.OnSelected();
        if (OnDataSelected != null)
        {
            OnDataSelected.Invoke(this);
        }
    }
}