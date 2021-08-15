using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class DialogueParser : MonoBehaviour {
   public string mInput;

   private void Start() {
      // mInput = "<tag>hello</tag>";
      mInput = "<root><tag>amazing <yeehaw>yeehaw </yeehaw></tag><cock>nice cock</cock></root>";
      Parser(mInput);
   }

   private void Parser(string input) {
      var xmlroot = XElement.Parse(input); 
      // var firstNodeContent = ((XElement)xmlroot.FirstNode).Value;
      // var secondNodeContent = ((XElement)xmlroot.NextNode).Value;
      // Debug.Log(firstNodeContent);
      // Debug.Log(secondNodeContent);

      foreach (var element in xmlroot.Elements()) {
         Debug.Log(element.Value); 
      }

      // var parser = XDocument.Parse(input);
      // Debug.Log(parser);
      // foreach (var element in parser.Nodes()) {
      //    Debug.Log(element);
      // }

      // foreach (var element in parser.Elements()) {
      //    Debug.Log(element.Name + ": " + element.Value);
      // }

      // Debug.Log(parser.Element("tag"));
      // Debug.Log(parser.Element("tag")?.Value);
      // Debug.Log(parser.Element("cock")?.Value);
   }

   // private void OnGUI() {
   //    if (GUI.Button(new Rect(10, 70, 50, 30), "Click")) {
   //       Parser(mInput);
   //    }
   // }
}
