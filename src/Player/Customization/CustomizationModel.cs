using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using pcrpg.src.Database.Models;
using pcrpg.src.Player.Utils;

namespace pcrpg.src.Player.Customization
{
    public class CustomizationModel : Script
    {
        public void InitializePedFace(NetHandle ent)
        {
            Client player = API.getPlayerFromHandle(ent);

            if (player == null) return;
            else if (!player.isLogged()) return;

            Character character = Data.Character[player];

            API.setEntitySyncedData(ent, "GTAO_HAS_CHARACTER_DATA", true);            

            API.setEntitySyncedData(ent, "GTAO_SHAPE_FIRST_ID", character.Trait.FaceFirst);
            API.setEntitySyncedData(ent, "GTAO_SHAPE_SECOND_ID", character.Trait.FaceSecond);
            API.setEntitySyncedData(ent, "GTAO_SKIN_FIRST_ID", character.Trait.SkinFirst);
            API.setEntitySyncedData(ent, "GTAO_SKIN_SECOND_ID", character.Trait.SkinSecond);
            API.setEntitySyncedData(ent, "GTAO_SHAPE_MIX", character.Trait.FaceMix);
            API.setEntitySyncedData(ent, "GTAO_SKIN_MIX", character.Trait.SkinMix);
            API.setEntitySyncedData(ent, "GTAO_HAIR", character.Trait.HairType);
            API.setEntitySyncedData(ent, "GTAO_HAIR_COLOR", character.Trait.HairColor);
            API.setEntitySyncedData(ent, "GTAO_HAIR_HIGHLIGHT_COLOR", character.Trait.HairHighlight);
            API.setEntitySyncedData(ent, "GTAO_EYE_COLOR", character.Trait.EyeColor);
            API.setEntitySyncedData(ent, "GTAO_EYEBROWS", character.Trait.Eyebrows);

            if (character.Trait.Beard != null)
                API.setEntitySyncedData(ent, "GTAO_BEARD", character.Trait.Beard); // No beard by default.

            if (character.Trait.Makeup != null)
                API.setEntitySyncedData(ent, "GTAO_MAKEUP", character.Trait.Makeup); // No makeup by default.

            if (character.Trait.Lipstick != null)
                API.setEntitySyncedData(ent, "GTAO_LIPSTICK", character.Trait.Lipstick); // No lipstick by default.

            API.setEntitySyncedData(ent, "GTAO_BEARD_COLOR", character.Trait.BeardColor);
            API.setEntitySyncedData(ent, "GTAO_BEARD_COLOR2", character.Trait.BeardColor);
            API.setEntitySyncedData(ent, "GTAO_EYEBROWS_COLOR", character.Trait.EyebrowsColor1);
            API.setEntitySyncedData(ent, "GTAO_EYEBROWS_COLOR2", character.Trait.EyebrowsColor2);
            API.setEntitySyncedData(ent, "GTAO_LIPSTICK_COLOR", character.Trait.LipstickColor);
            API.setEntitySyncedData(ent, "GTAO_LIPSTICK_COLOR2", character.Trait.LipstickColor);
            API.setEntitySyncedData(ent, "GTAO_MAKEUP_COLOR", character.Trait.MakeupColor);
            API.setEntitySyncedData(ent, "GTAO_MAKEUP_COLOR2", character.Trait.MakeupColor);

            var list = new float[21];

            for (var i = 0; i < 21; i++)
            {
                list[i] = 0f;
            }

            API.setEntitySyncedData(ent, "GTAO_FACE_FEATURES_LIST", list);

            foreach (var clothes in character.Clothes)
            {
                if (clothes.IsAccessory == true) continue;

                switch (clothes.BodyPart)
                {
                    case 3:
                        // Torso
                        API.setEntitySyncedData(ent, "GTAO_TORSO", clothes.Variation);
                        break;
                    case 4:
                        // Legs
                        API.setEntitySyncedData(ent, "GTAO_LEGS", clothes.Variation);
                        break;
                    case 5:
                        // Bags
                        API.setEntitySyncedData(ent, "GTAO_BACKPACK", clothes.Variation);
                        break;
                    case 6:
                        // Feet
                        API.setEntitySyncedData(ent, "GTAO_FEET", clothes.Variation);
                        break;
                    case 7:
                        // Accessory
                        API.setEntitySyncedData(ent, "GTAO_ACCESSORY", clothes.Variation);
                        break;
                    case 8:
                        // Undershirt
                        API.setEntitySyncedData(ent, "GTAO_UNDERSHIRT", clothes.Variation);
                        break;
                    case 11:
                        // Top
                        API.setEntitySyncedData(ent, "GTAO_TOP", clothes.Variation);
                        API.setEntitySyncedData(ent, "GTAO_TOP_TEXTURE", clothes.Texture);
                        break;
                }
            }
        }

        public void RemovePedFace(NetHandle ent)
        {
            API.setEntitySyncedData(ent, "GTAO_HAS_CHARACTER_DATA", false);

            API.resetEntitySyncedData(ent, "GTAO_SHAPE_FIRST_ID");
            API.resetEntitySyncedData(ent, "GTAO_SHAPE_SECOND_ID");
            API.resetEntitySyncedData(ent, "GTAO_SKIN_FIRST_ID");
            API.resetEntitySyncedData(ent, "GTAO_SKIN_SECOND_ID");
            API.resetEntitySyncedData(ent, "GTAO_SHAPE_MIX");
            API.resetEntitySyncedData(ent, "GTAO_SKIN_MIX");
            API.resetEntitySyncedData(ent, "GTAO_HAIR");
            API.resetEntitySyncedData(ent, "GTAO_HAIR_COLOR");
            API.resetEntitySyncedData(ent, "GTAO_HAIR_HIGHLIGHT_COLOR");
            API.resetEntitySyncedData(ent, "GTAO_EYE_COLOR");
            API.resetEntitySyncedData(ent, "GTAO_EYEBROWS");
            API.resetEntitySyncedData(ent, "GTAO_BEARD");
            API.resetEntitySyncedData(ent, "GTAO_MAKEUP");
            API.resetEntitySyncedData(ent, "GTAO_LIPSTICK");
            API.resetEntitySyncedData(ent, "GTAO_BEARD_COLOR");
            API.resetEntitySyncedData(ent, "GTAO_BEARD_COLOR2");
            API.resetEntitySyncedData(ent, "GTAO_EYEBROWS_COLOR");
            API.resetEntitySyncedData(ent, "GTAO_EYEBROWS_COLOR2");
            API.resetEntitySyncedData(ent, "GTAO_MAKEUP_COLOR");
            API.resetEntitySyncedData(ent, "GTAO_MAKEUP_COLOR2");
            API.resetEntitySyncedData(ent, "GTAO_LIPSTICK_COLOR");
            API.resetEntitySyncedData(ent, "GTAO_LIPSTICK_COLOR2");
            API.resetEntitySyncedData(ent, "GTAO_FACE_FEATURES_LIST");
            API.resetEntitySyncedData(ent, "GTAO_TORSO");
            API.resetEntitySyncedData(ent, "GTAO_LEGS");
            API.resetEntitySyncedData(ent, "GTAO_FEET");
            API.resetEntitySyncedData(ent, "GTAO_UNDERSHIRT");
            API.resetEntitySyncedData(ent, "GTAO_TOP");
            API.resetEntitySyncedData(ent, "GTAO_TOP_TEXTURE");
            API.resetEntitySyncedData(ent, "GTAO_ACCESSORY");
            API.resetEntitySyncedData(ent, "GTAO_BACKPACK");
        }

        public bool IsPlayerFaceValid(NetHandle ent)
        {
            if (!API.hasEntitySyncedData(ent, "GTAO_SHAPE_FIRST_ID")) return false;
            if (!API.hasEntitySyncedData(ent, "GTAO_SHAPE_SECOND_ID")) return false;
            if (!API.hasEntitySyncedData(ent, "GTAO_SKIN_FIRST_ID")) return false;
            if (!API.hasEntitySyncedData(ent, "GTAO_SKIN_SECOND_ID")) return false;
            if (!API.hasEntitySyncedData(ent, "GTAO_SHAPE_MIX")) return false;
            if (!API.hasEntitySyncedData(ent, "GTAO_SKIN_MIX")) return false;
            if (!API.hasEntitySyncedData(ent, "GTAO_HAIR")) return false;
            if (!API.hasEntitySyncedData(ent, "GTAO_HAIR_COLOR")) return false;
            if (!API.hasEntitySyncedData(ent, "GTAO_HAIR_HIGHLIGHT_COLOR")) return false;
            if (!API.hasEntitySyncedData(ent, "GTAO_EYE_COLOR")) return false;
            if (!API.hasEntitySyncedData(ent, "GTAO_EYEBROWS")) return false;
            //if (!API.hasEntitySyncedData(ent, "GTAO_BEARD")) return false; // Player may have no beard
            //if (!API.hasEntitySyncedData(ent, "GTAO_MAKEUP")) return false; // Player may have no makeup
            //if (!API.hasEntitySyncedData(ent, "GTAO_LIPSTICK")) return false; // Player may have no lipstick
            if (!API.hasEntitySyncedData(ent, "GTAO_BEARD_COLOR")) return false;
            if (!API.hasEntitySyncedData(ent, "GTAO_BEARD_COLOR2")) return false;
            if (!API.hasEntitySyncedData(ent, "GTAO_EYEBROWS_COLOR")) return false;
            if (!API.hasEntitySyncedData(ent, "GTAO_EYEBROWS_COLOR2")) return false;
            if (!API.hasEntitySyncedData(ent, "GTAO_MAKEUP_COLOR")) return false;
            if (!API.hasEntitySyncedData(ent, "GTAO_MAKEUP_COLOR2")) return false;
            if (!API.hasEntitySyncedData(ent, "GTAO_LIPSTICK_COLOR")) return false;
            if (!API.hasEntitySyncedData(ent, "GTAO_LIPSTICK_COLOR2")) return false;
            if (!API.hasEntitySyncedData(ent, "GTAO_FACE_FEATURES_LIST")) return false;
            if (!API.hasEntitySyncedData(ent, "GTAO_TORSO")) return false;
            if (!API.hasEntitySyncedData(ent, "GTAO_LEGS")) return false;
            if (!API.hasEntitySyncedData(ent, "GTAO_FEET")) return false;
            if (!API.hasEntitySyncedData(ent, "GTAO_UNDERSHIRT")) return false;
            if (!API.hasEntitySyncedData(ent, "GTAO_TOP")) return false;
            if (!API.hasEntitySyncedData(ent, "GTAO_TOP_TEXTURE")) return false;
            //if (!API.hasEntitySyncedData(ent, "GTAO_ACCESSORY")) return false; // Player may have no accessory
            //if (!API.hasEntitySyncedData(ent, "GTAO_BACKPACK")) return false; // Player may have no backpack

            return true;
        }

        public void UpdatePlayerFace(NetHandle player)
        {
            API.triggerClientEventForAll("UPDATE_CHARACTER", player);
        }
    }
}
