﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using System.Text;
using GeometryGym.Ifc;

namespace IFC
{
	internal abstract class IFCExampleBase
	{
		internal void GenerateExample(string path,ModelView modelView) 
		{
			DatabaseIfc database = new DatabaseIfc(false, modelView);
		 	database.Factory.Options.GenerateOwnerHistory = false;
			database.NextObjectRecord = 10;
			IfcBuilding building = new IfcBuilding(database, "IfcBuilding") { GlobalId = "39t4Pu3nTC4ekXYRIHJB9W"};
			building.ContainsElements[0].GlobalId = "3Sa3dTJGn0H8TQIGiuGQd5";
			building.Comments.Add("defines the default building (as required as the minimum spatial element) ");
			database.NextObjectRecord = 20;
			IfcProject project = new IfcProject(building, "IfcProject", IfcUnitAssignment.Length.Millimetre) { GlobalId = "0$WU4A9R19$vKWO$AdOnKA"};
			project.IsDecomposedBy[0].GlobalId = "091a6ewbvCMQ2Vyiqspa7a";
			project.Comments.Add("general entities required for all IFC data sets, defining the context for the exchange");
			database.Factory.SubContext(FactoryIfc.SubContextIdentifier.Body);
			database.NextObjectRecord = 50;
			GenerateData(database,building);
			List<IfcRelDeclares> rds = project.Declares;
			
			//Unique ids assigned to generate constant IfcScript  sample files, remove otherwise
			if (rds.Count > 0)
				rds[0].GlobalId = "1Cjr05W9T0fx0M3_mdVqMd";

			database[50].Comments.Add("Example data for " + this.GetType().Name);


			string filePath = Path.Combine(path,this.GetType().Name + ".ifc");
			if (File.Exists(filePath))
			{
				string[] newLines = database.ToString().Split(new char[] { '\r' , '\n' },StringSplitOptions.RemoveEmptyEntries);
				List<string> existingLines = new List<string>( File.ReadAllLines(filePath));
				existingLines.RemoveAll(x => string.IsNullOrEmpty(x));
				
				if (newLines.Length == existingLines.Count)
				{
					bool identical = true;
					int icounter = 0;
					for(; icounter < newLines.Length; icounter++)
					{
						string s1 = newLines[icounter];
						if (s1.StartsWith("FILE_SCHEMA"))
							break;
					}
					for (; icounter < newLines.Length; icounter++)
					{
						string s1 = newLines[icounter], s2 = existingLines[icounter];
						if (s1.StartsWith("/* time_stamp */ ") && s2.StartsWith("/* time_stamp */ "))
							continue;
						if (string.Compare(s1, s2, true) != 0)
						{
							identical = false;
							break;
						}
					}
					if (identical)
						return;
				}
			}
			database.WriteFile(filePath); 
		}
		protected virtual void GenerateData(DatabaseIfc database,IfcBuilding building) { }

		protected IfcBeamType GetParametericIPE200(DatabaseIfc database)
		{
			IfcMaterialProfile materialProfile = GetParametericIPE200Profile(database);
			IfcBeamType beamType = new IfcBeamType(materialProfile.Name, materialProfile, IfcBeamTypeEnum.JOIST) { GlobalId = "32b2OtzCP30umNyY5LsCfN" };
			database.Context.AddDeclared(beamType);

			//Unique ids assigned to generate constant IfcScript  sample files, remove otherwise
			beamType.ObjectTypeOf.GlobalId = "3s_DqAVvb3LguudTShJHVo";
			beamType.MaterialSelect.Associates.GlobalId = "0NkGSIHVT3SeAR6bnw7pSa";

			return beamType;
		}
		protected IfcMaterialProfile GetParametericIPE200Profile(DatabaseIfc database)
		{
			IfcMaterial material = new IfcMaterial(database, "S355JR") { Category = "Steel" };

			//Unique ids assigned to generate constant IfcScript  sample files, remove otherwise
			material.Associates.GlobalId = "1oJeVe14nCYf5cL0Mka0KL";

			string name = "IPE200";
			IfcIShapeProfileDef ipe200 = new IfcIShapeProfileDef(database, name, 200, 100, 5.6, 8.5, 12);
			return new IfcMaterialProfile(name, material, ipe200);
		}
	}
}
