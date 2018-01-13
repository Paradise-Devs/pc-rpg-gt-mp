using pcrpg.src.Database.Models;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using System;
using System.Linq;

namespace pcrpg.src.Player.Selection
{
    class CharacterCreation : Script
    {
        public CharacterCreation()
        {
            API.onClientEventTrigger += OnClientTriggerEvent;
        }

        private void OnClientTriggerEvent(Client sender, string eventName, object[] arguments)
        {
            if (eventName == "FinishCharacterCreation")
            {
                if (!API.hasEntityData(sender, "User"))
                    return;

                User user = API.getEntityData(sender, "User");
                var characterData = API.fromJson((string)arguments[0]);

                using (var ctx = new ContextFactory().Create())
                {
                    // Checking if name is available
                    string characterName = null;
                    characterName = characterData.name;
                    var isNameTaken = ctx.Characters.FirstOrDefault(up => up.Name == characterName);
                    if (isNameTaken != null)
                    {
                        API.triggerClientEvent(sender, "CharacterNameAlreadyTaken");
                        return;
                    }

                    // Character
                    Character character = new Character
                    {
                        UserId = user.Id,
                        Name = characterData.name,
                        Gender = characterData.gender,
                        Money = 0,
                        Bank = 8500,
                        Level = 1,
                        Experience = 0,
                        PlayedTime = 0,
                        PositionX = 169.3792f,
                        PositionY = -967.8402f,
                        PositionZ = 29.98808f,
                        RotationZ = 138.6666f,
                        LastLogin = DateTime.Now,
                        CreatedAt = DateTime.Now
                    };

                    ctx.Characters.Add(character);
                    Data.Character.Add(sender, character);

                    // Traits
                    CharacterTrait traits = new CharacterTrait
                    {
                        Character = character,
                        FaceFirst = characterData.faceFirst,
                        FaceSecond = characterData.faceSecond,
                        FaceMix = characterData.faceMix,
                        SkinFirst = characterData.skinFirst,
                        SkinSecond = characterData.skinSecond,
                        SkinMix = characterData.skinMix,
                        HairType = characterData.hairType,
                        HairColor = characterData.hairColor,
                        HairHighlight = characterData.hairHighlight,
                        EyeColor = characterData.eyeColor,
                        Eyebrows = characterData.eyebrows,
                        EyebrowsColor1 = characterData.eyebrowsColor1,
                        EyebrowsColor2 = characterData.eyebrowsColor2,
                        Beard = characterData.beard,
                        BeardColor = characterData.beardColor,
                        Makeup = characterData.makeup,
                        MakeupColor = characterData.makeupColor,
                        Lipstick = characterData.lipstick,
                        LipstickColor = characterData.lipstickColor
                    };

                    ctx.Traits.Add(traits);

                    // Clothes
                    CharacterClothes torso = new CharacterClothes
                    {
                        CharacterId = character.Id,
                        BodyPart = 3,
                        Variation = characterData.torso,
                        Texture = 0,
                        IsAccessory = false
                    };

                    CharacterClothes top = new CharacterClothes
                    {
                        CharacterId = character.Id,
                        BodyPart = 11,
                        Variation = characterData.topshirt,
                        Texture = characterData.topshirtTexture,
                        IsAccessory = false
                    };

                    CharacterClothes undershirt = new CharacterClothes
                    {
                        CharacterId = character.Id,
                        BodyPart = 8,
                        Variation = characterData.undershirt,
                        Texture = 0,
                        IsAccessory = false
                    };

                    CharacterClothes pants = new CharacterClothes
                    {
                        CharacterId = character.Id,
                        BodyPart = 4,
                        Variation = characterData.legs,
                        Texture = 0,
                        IsAccessory = false
                    };

                    CharacterClothes shoes = new CharacterClothes
                    {
                        CharacterId = character.Id,
                        BodyPart = 6,
                        Variation = characterData.feet,
                        Texture = 0,
                        IsAccessory = false
                    };

                    CharacterClothes accessory = new CharacterClothes
                    {
                        CharacterId = character.Id,
                        BodyPart = 7,
                        Variation = characterData.accessory,
                        Texture = 0,
                        IsAccessory = false
                    };

                    ctx.Clothes.Add(torso);
                    ctx.Clothes.Add(top);
                    ctx.Clothes.Add(undershirt);
                    ctx.Clothes.Add(pants);
                    ctx.Clothes.Add(shoes);
                    ctx.Clothes.Add(accessory);

                    sender.name = characterData.name;

                    // Send to spawn
                    API.triggerClientEvent(sender, "closeCharacterCreationBrowser");

                    Managers.DimensionManager.DismissPrivateDimension(sender);
                    API.setEntityDimension(sender, 0);

                    ctx.SaveChanges();

                    // Sync player face with other players
                    Customization.CustomizationModel gtao = new Customization.CustomizationModel();
                    gtao.InitializePedFace(sender);
                    gtao.UpdatePlayerFace(sender);
                }                
            }
        }
    }
}
