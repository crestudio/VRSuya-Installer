using UnityEngine;

using VRC.SDK3.Dynamics.PhysBone.Components;

/*
 * VRSuya Modular Component
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace VRSuya.Modular {

	public enum PhysBoneType {
		Cheek, Toe
	}

	[AddComponentMenu("VRSuya/Modular/VRSuya PhysBone Connector")]
	[HelpURL("https://vrsuya.booth.pm/")]
	public class PhysBoneConnector : MonoBehaviour {
		public PhysBoneType TargetType = PhysBoneType.Cheek;
		public VRCPhysBone TargetCheek_L;
		public VRCPhysBone TargetCheek_R;
		public VRCPhysBone TargetThumbToe1_L;
		public VRCPhysBone TargetThumbToe1_R;
		public VRCPhysBone TargetIndexToe1_L;
		public VRCPhysBone TargetIndexToe1_R;
		public VRCPhysBone TargetMiddleToe1_L;
		public VRCPhysBone TargetMiddleToe1_R;
		public VRCPhysBone TargetRingToe1_L;
		public VRCPhysBone TargetRingToe1_R;
		public VRCPhysBone TargetLittleToe1_L;
		public VRCPhysBone TargetLittleToe1_R;
	}
}