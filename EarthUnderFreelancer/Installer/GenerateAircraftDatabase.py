#!/usr/bin/env python3
"""
Aircraft Database Generator - Creates 500+ aircraft entries for EarthUnderFreelancer
"""

def generate_wwii_axis_fighters():
    """Generate German, Italian, and Japanese WWII fighters"""
    aircraft_list = []
    
    # Messerschmitt Bf 109 variants (20 variants)
    bf109_variants = ['E-1', 'E-3', 'E-4', 'E-7', 'F-1', 'F-2', 'F-4', 'G-2', 'G-4', 'G-6', 'G-10', 'G-14', 'K-4']
    for variant in bf109_variants:
        speed = 570 if 'K' in variant else (640 if 'G' in variant else 570)
        aircraft_list.append(f'''
                new AircraftData
                {{
                    Name = "Messerschmitt Bf 109",
                    Variant = "{variant}",
                    Country = "Germany",
                    Manufacturer = "Messerschmitt",
                    YearIntroduced = {1939 if 'E' in variant else (1941 if 'F' in variant else (1942 if 'G' in variant else 1944))},
                    Generation = AircraftGeneration.WWII,
                    Role = AircraftRole.Fighter,
                    Description = "Backbone of Luftwaffe fighter force",
                    MaxSpeed = {speed},
                    CruiseSpeed = {speed - 100},
                    MaxAltitude = {11000 if 'K' in variant or 'G' in variant and '10' in variant else 10500},
                    RateOfClimb = {17.0 if 'K' in variant else 15.5}f,
                    Range = {850 if 'K' in variant or 'G-10' in variant else 700},
                    ServiceCeiling = {12000 if 'K' in variant else 11000},
                    Length = 8.95f,
                    Wingspan = 9.92f,
                    Height = 2.60f,
                    EmptyWeight = {2247 if 'K' in variant else (2400 if 'G' in variant else 2100)},
                    MaxTakeoffWeight = {3400 if 'K' in variant else (3150 if 'G' in variant else 2900)},
                    FuelCapacity = {400 if 'K' in variant or 'G' in variant else 320},
                    Crew = 1,
                    ArmorRating = {48 if 'K' in variant or 'G-10' in variant else 42},
                    Maneuverability = {86 if 'K' in variant else 84},
                    Weapons = new List<WeaponSystem>
                    {{
                        new WeaponSystem {{ Name = "MG 151/20 20mm", Type = "Gun", Quantity = {3 if 'K' in variant else (1 if 'F' in variant else 2)}, Damage = 42, Range = 1100, RateOfFire = 750, Accuracy = 84 }},
                        new WeaponSystem {{ Name = "MG 131 13mm", Type = "Gun", Quantity = 2, Damage = 28, Range = 900, RateOfFire = 900, Accuracy = 80 }}
                    }},
                    PurchasePrice = {92000 if 'K' in variant else (75000 if 'G' in variant else 62000)},
                    MaintenanceCost = {3000 if 'K' in variant else 2500},
                    RepairCost = {9200 if 'K' in variant else 7500},
                    UnlockLevel = {18 if 'K' in variant else (15 if 'G' in variant else 12)}
                }},''')
    
    # Focke-Wulf Fw 190 variants (15 variants)
    fw190_variants = ['A-1', 'A-2', 'A-3', 'A-4', 'A-5', 'A-6', 'A-7', 'A-8', 'D-9', 'D-11', 'D-12', 'D-13', 'F-8']
    for variant in fw190_variants:
        is_dora = 'D-' in variant
        speed = 685 if is_dora else 656
        aircraft_list.append(f'''
                new AircraftData
                {{
                    Name = "Focke-Wulf Fw 190",
                    Variant = "{variant}",
                    Country = "Germany",
                    Manufacturer = "Focke-Wulf",
                    YearIntroduced = {1944 if is_dora else (1943 if 'A-8' in variant or 'A-7' in variant else 1941)},
                    Generation = AircraftGeneration.WWII,
                    Role = AircraftRole.{"Fighter" if is_dora else "FighterBomber"},
                    Description = "Radial-engine fighter with excellent performance",
                    MaxSpeed = {speed},
                    CruiseSpeed = {speed - 120},
                    MaxAltitude = {12000 if is_dora else 11000},
                    RateOfClimb = {18.5 if is_dora else 15.0}f,
                    Range = {835 if is_dora else 800},
                    ServiceCeiling = {12100 if is_dora else 11050},
                    Length = {10.19 if is_dora else 8.95}f,
                    Wingspan = 10.51f,
                    Height = 3.95f,
                    EmptyWeight = {3490 if is_dora else 3200},
                    MaxTakeoffWeight = {4840 if is_dora else 4417},
                    FuelCapacity = {525 if is_dora else 480},
                    Crew = 1,
                    ArmorRating = {52 if is_dora else 50},
                    Maneuverability = {84 if is_dora else 78},
                    Weapons = new List<WeaponSystem>
                    {{
                        new WeaponSystem {{ Name = "MG 151/20 20mm", Type = "Gun", Quantity = {2 if is_dora else 4}, Damage = 42, Range = 1100, RateOfFire = 750, Accuracy = 86 }},
                        new WeaponSystem {{ Name = "MG 131 13mm", Type = "Gun", Quantity = 2, Damage = 28, Range = 900, RateOfFire = 900, Accuracy = 82 }},
                        new WeaponSystem {{ Name = "Bombs", Type = "Bomb", Quantity = {1 if is_dora else 1}, Damage = {500 if is_dora else 500}, Range = 0, RateOfFire = 0, Accuracy = 70 }}
                    }},
                    PurchasePrice = {98000 if is_dora else 85000},
                    MaintenanceCost = {3200 if is_dora else 2800},
                    RepairCost = {9800 if is_dora else 8500},
                    UnlockLevel = {18 if is_dora else 16}
                }},''')
    
    # Japanese fighters - A6M Zero variants
    zero_variants = ['A6M2', 'A6M3', 'A6M5', 'A6M5c', 'A6M6c']
    for variant in zero_variants:
        late = '5' in variant or '6' in variant
        aircraft_list.append(f'''
                new AircraftData
                {{
                    Name = "Mitsubishi A6M Zero",
                    Variant = "{variant}",
                    Country = "Japan",
                    Manufacturer = "Mitsubishi",
                    YearIntroduced = {1943 if late else 1940},
                    Generation = AircraftGeneration.WWII,
                    Role = AircraftRole.Fighter,
                    Description = "Legendary Japanese carrier fighter, extreme maneuverability",
                    MaxSpeed = {565 if late else 534},
                    CruiseSpeed = {420 if late else 370},
                    MaxAltitude = {11740 if late else 10000},
                    RateOfClimb = {15.7 if late else 15.5}f,
                    Range = {1920 if late else 1929},
                    ServiceCeiling = {11750 if late else 10050},
                    Length = 9.06f,
                    Wingspan = 11.00f if '5' in variant else 12.00f,
                    Height = 3.05f,
                    EmptyWeight = {1894 if late else 1680},
                    MaxTakeoffWeight = {2733 if late else 2410},
                    FuelCapacity = {525 if late else 518},
                    Crew = 1,
                    ArmorRating = {28 if late else 18},
                    Maneuverability = {96 if not late else 94},
                    Weapons = new List<WeaponSystem>
                    {{
                        new WeaponSystem {{ Name = "Type 99 20mm", Type = "Gun", Quantity = 2, Damage = 40, Range = 1000, RateOfFire = 520, Accuracy = 80 }},
                        new WeaponSystem {{ Name = "Type 97 7.7mm", Type = "Gun", Quantity = 2, Damage = 16, Range = 700, RateOfFire = 900, Accuracy = 75 }},
                        new WeaponSystem {{ Name = "Bombs", Type = "Bomb", Quantity = 2, Damage = 60, Range = 0, RateOfFire = 0, Accuracy = 65 }}
                    }},
                    PurchasePrice = {68000 if late else 55000},
                    MaintenanceCost = {2200 if late else 1800},
                    RepairCost = {6800 if late else 5500},
                    UnlockLevel = {14 if late else 11}
                }},''')
    
    # Ki-84 variants
    ki84_variants = ['Ki-84-Ia', 'Ki-84-Ib', 'Ki-84-Ic']
    for variant in ki84_variants:
        aircraft_list.append(f'''
                new AircraftData
                {{
                    Name = "Nakajima Ki-84",
                    Variant = "{variant}",
                    Country = "Japan",
                    Manufacturer = "Nakajima",
                    YearIntroduced = 1944,
                    Generation = AircraftGeneration.WWII,
                    Role = AircraftRole.Fighter,
                    Description = "Best Japanese fighter of WWII",
                    MaxSpeed = 686,
                    CruiseSpeed = 420,
                    MaxAltitude = 10500,
                    RateOfClimb = 21.5f,
                    Range = 2168,
                    ServiceCeiling = 10540,
                    Length = 9.92f,
                    Wingspan = 11.23f,
                    Height = 3.39f,
                    EmptyWeight = 2660,
                    MaxTakeoffWeight = 4170,
                    FuelCapacity = 620,
                    Crew = 1,
                    ArmorRating = 45,
                    Maneuverability = 88,
                    Weapons = new List<WeaponSystem>
                    {{
                        new WeaponSystem {{ Name = "Ho-5 20mm", Type = "Gun", Quantity = {'4' if 'Ic' in variant else '2'}, Damage = 42, Range = 1050, RateOfFire = 850, Accuracy = 85 }},
                        new WeaponSystem {{ Name = "Ho-103 12.7mm", Type = "Gun", Quantity = {'0' if 'Ic' in variant else '2'}, Damage = 26, Range = 850, RateOfFire = 900, Accuracy = 82 }},
                        new WeaponSystem {{ Name = "Bombs", Type = "Bomb", Quantity = 2, Damage = 250, Range = 0, RateOfFire = 0, Accuracy = 70 }}
                    }},
                    PurchasePrice = 89000,
                    MaintenanceCost = 2900,
                    RepairCost = 8900,
                    UnlockLevel = 17
                }},''')
    
    # Soviet fighters - Yak variants
    yak_variants = ['Yak-1', 'Yak-1b', 'Yak-3', 'Yak-7B', 'Yak-9', 'Yak-9D', 'Yak-9T', 'Yak-9U', 'Yak-9P']
    for variant in yak_variants:
        late = '9U' in variant or '9P' in variant or '3' in variant
        aircraft_list.append(f'''
                new AircraftData
                {{
                    Name = "Yakovlev {variant.split('-')[0]}",
                    Variant = "{variant.split('-')[1] if '-' in variant else ''}",
                    Country = "Soviet Union",
                    Manufacturer = "Yakovlev",
                    YearIntroduced = {1945 if '9P' in variant else (1944 if '9U' in variant or '3' in variant else (1942 if '9' in variant or '7' in variant else 1940))},
                    Generation = AircraftGeneration.WWII,
                    Role = AircraftRole.Fighter,
                    Description = "Light and nimble Soviet fighter",
                    MaxSpeed = {672 if late else (651 if '9' in variant else 592)},
                    CruiseSpeed = {500 if late else 450},
                    MaxAltitude = {11800 if late else 10650},
                    RateOfClimb = {18.5 if late else 16.7}f,
                    Range = {1360 if '9D' in variant else (875 if late else 700)},
                    ServiceCeiling = {11850 if late else 10700},
                    Length = {8.5 if '3' in variant else 8.67}f,
                    Wingspan = {9.2 if '3' in variant else 9.74}f,
                    Height = 2.44f,
                    EmptyWeight = {2105 if '3' in variant else (2512 if late else 2350)},
                    MaxTakeoffWeight = {2692 if '3' in variant else (3204 if late else 2883)},
                    FuelCapacity = {320 if '3' in variant else 440},
                    Crew = 1,
                    ArmorRating = {38 if late else 32},
                    Maneuverability = {92 if '3' in variant else (88 if late else 85)},
                    Weapons = new List<WeaponSystem>
                    {{
                        new WeaponSystem {{ Name = "{'NS-37 37mm' if '9T' in variant else 'ShVAK 20mm'}", Type = "Gun", Quantity = {'1' if '9T' in variant else '1'}, Damage = {'95' if '9T' in variant else '42'}, Range = {'600' if '9T' in variant else '1000'}, RateOfFire = {'250' if '9T' in variant else '800'}, Accuracy = {'78' if '9T' in variant else '84'} }},
                        new WeaponSystem {{ Name = "UBS 12.7mm", Type = "Gun", Quantity = {'1' if '9T' in variant or '3' in variant else '2'}, Damage = 26, Range = 800, RateOfFire = 1000, Accuracy = 80 }}
                    }},
                    PurchasePrice = {82000 if late else (68000 if '9' in variant else 58000)},
                    MaintenanceCost = {2700 if late else 2200},
                    RepairCost = {8200 if late else 6800},
                    UnlockLevel = {16 if late else (13 if '9' in variant else 11)}
                }},''')
    
    # La-5 and La-7 variants
    la_variants = ['La-5', 'La-5F', 'La-5FN', 'La-7', 'La-7-3']
    for variant in la_variants:
        is_la7 = '7' in variant
        aircraft_list.append(f'''
                new AircraftData
                {{
                    Name = "Lavochkin {'La-7' if is_la7 else 'La-5'}",
                    Variant = "{variant.replace('La-5', '').replace('La-7', '')}",
                    Country = "Soviet Union",
                    Manufacturer = "Lavochkin",
                    YearIntroduced = {1944 if is_la7 else 1942},
                    Generation = AircraftGeneration.WWII,
                    Role = AircraftRole.Fighter,
                    Description = "Powerful radial-engine Soviet fighter",
                    MaxSpeed = {661 if is_la7 else (634 if 'FN' in variant else 580)},
                    CruiseSpeed = {500 if is_la7 else 450},
                    MaxAltitude = {10450 if is_la7 else 9500},
                    RateOfClimb = {20.0 if is_la7 else 16.7}f,
                    Range = {665 if is_la7 else 765},
                    ServiceCeiling = {10500 if is_la7 else 9550},
                    Length = {8.67 if is_la7 else 8.46}f,
                    Wingspan = 9.80f,
                    Height = 2.54f,
                    EmptyWeight = {2638 if is_la7 else 2605},
                    MaxTakeoffWeight = {3400 if is_la7 else 3360},
                    FuelCapacity = 370,
                    Crew = 1,
                    ArmorRating = {42 if is_la7 else 38},
                    Maneuverability = {86 if is_la7 else 82},
                    Weapons = new List<WeaponSystem>
                    {{
                        new WeaponSystem {{ Name = "{'B-20 20mm' if is_la7 else 'ShVAK 20mm'}", Type = "Gun", Quantity = {'3' if '7-3' in variant else '2'}, Damage = 42, Range = 1050, RateOfFire = 800, Accuracy = 85 }}
                    }},
                    PurchasePrice = {85000 if is_la7 else 70000},
                    MaintenanceCost = {2800 if is_la7 else 2300},
                    RepairCost = {8500 if is_la7 else 7000},
                    UnlockLevel = {17 if is_la7 else 14}
                }},''')
    
    return '\n'.join(aircraft_list)

def generate_wwii_bombers():
    """Generate WWII bomber aircraft"""
    aircraft_list = []
    
    # B-17 variants
    b17_variants = ['B-17E', 'B-17F', 'B-17G']
    for variant in b17_variants:
        is_g = 'G' in variant
        aircraft_list.append(f'''
                new AircraftData
                {{
                    Name = "Boeing B-17 Flying Fortress",
                    Variant = "{variant.replace('B-17', '')}",
                    Country = "United States",
                    Manufacturer = "Boeing",
                    YearIntroduced = {1943 if is_g else (1942 if 'F' in variant else 1941)},
                    Generation = AircraftGeneration.WWII,
                    Role = AircraftRole.Bomber,
                    Description = "Legendary heavy bomber with defensive armament",
                    MaxSpeed = 462,
                    CruiseSpeed = 293,
                    MaxAltitude = 10850,
                    RateOfClimb = 4.3f,
                    Range = 3219,
                    ServiceCeiling = 10900,
                    Length = 22.66f,
                    Wingspan = 31.62f,
                    Height = 5.82f,
                    EmptyWeight = {16391 if is_g else 16206},
                    MaxTakeoffWeight = {29710 if is_g else 24500},
                    FuelCapacity = 6814,
                    Crew = 10,
                    ArmorRating = 75,
                    Maneuverability = 25,
                    Weapons = new List<WeaponSystem>
                    {{
                        new WeaponSystem {{ Name = "M2 .50 cal", Type = "Gun", Quantity = {13 if is_g else (11 if 'F' in variant else 9)}, Damage = 25, Range = 800, RateOfFire = 800, Accuracy = 65 }},
                        new WeaponSystem {{ Name = "Bombs", Type = "Bomb", Quantity = 8, Damage = 1000, Range = 0, RateOfFire = 0, Accuracy = 60 }}
                    }},
                    PurchasePrice = {180000 if is_g else 165000},
                    MaintenanceCost = 6000,
                    RepairCost = 18000,
                    UnlockLevel = {20 if is_g else 18}
                }},''')
    
    # B-24 Liberator variants
    b24_variants = ['B-24D', 'B-24J']
    for variant in b24_variants:
        aircraft_list.append(f'''
                new AircraftData
                {{
                    Name = "Consolidated B-24 Liberator",
                    Variant = "{variant.replace('B-24', '')}",
                    Country = "United States",
                    Manufacturer = "Consolidated",
                    YearIntroduced = {1944 if 'J' in variant else 1942},
                    Generation = AircraftGeneration.WWII,
                    Role = AircraftRole.Bomber,
                    Description = "Long-range heavy bomber with high wing",
                    MaxSpeed = 467,
                    CruiseSpeed = 346,
                    MaxAltitude = 8535,
                    RateOfClimb = 5.2f,
                    Range = 3380,
                    ServiceCeiling = 8600,
                    Length = 20.47f,
                    Wingspan = 33.53f,
                    Height = 5.49f,
                    EmptyWeight = 16556,
                    MaxTakeoffWeight = 29484,
                    FuelCapacity = 10814,
                    Crew = 10,
                    ArmorRating = 72,
                    Maneuverability = 23,
                    Weapons = new List<WeaponSystem>
                    {{
                        new WeaponSystem {{ Name = "M2 .50 cal", Type = "Gun", Quantity = 10, Damage = 25, Range = 800, RateOfFire = 800, Accuracy = 63 }},
                        new WeaponSystem {{ Name = "Bombs", Type = "Bomb", Quantity = 8, Damage = 1000, Range = 0, RateOfFire = 0, Accuracy = 62 }}
                    }},
                    PurchasePrice = {175000 if 'J' in variant else 160000},
                    MaintenanceCost = 5800,
                    RepairCost = 17500,
                    UnlockLevel = {19 if 'J' in variant else 17}
                }},''')
    
    # B-29 Superfortress
    aircraft_list.append('''
                new AircraftData
                {
                    Name = "Boeing B-29 Superfortress",
                    Variant = "",
                    Country = "United States",
                    Manufacturer = "Boeing",
                    YearIntroduced = 1944,
                    Generation = AircraftGeneration.WWII,
                    Role = AircraftRole.StrategicBomber,
                    Description = "Advanced pressurized heavy bomber",
                    MaxSpeed = 574,
                    CruiseSpeed = 370,
                    MaxAltitude = 11430,
                    RateOfClimb = 5.4f,
                    Range = 5230,
                    ServiceCeiling = 11500,
                    Length = 30.18f,
                    Wingspan = 43.05f,
                    Height = 8.46f,
                    EmptyWeight = 33795,
                    MaxTakeoffWeight = 60560,
                    FuelCapacity = 28348,
                    Crew = 11,
                    ArmorRating = 85,
                    Maneuverability = 20,
                    Weapons = new List<WeaponSystem>
                    {
                        new WeaponSystem { Name = "M2 .50 cal", Type = "Gun", Quantity = 12, Damage = 25, Range = 800, RateOfFire = 800, Accuracy = 70 },
                        new WeaponSystem { Name = "Bombs", Type = "Bomb", Quantity = 20, Damage = 1000, Range = 0, RateOfFire = 0, Accuracy = 70 }
                    },
                    PurchasePrice = 280000,
                    MaintenanceCost = 9000,
                    RepairCost = 28000,
                    UnlockLevel = 25
                },''')
    
    # Lancaster variants
    lancaster_variants = ['Mk I', 'Mk III', 'Mk X']
    for variant in lancaster_variants:
        aircraft_list.append(f'''
                new AircraftData
                {{
                    Name = "Avro Lancaster",
                    Variant = "{variant}",
                    Country = "United Kingdom",
                    Manufacturer = "Avro",
                    YearIntroduced = {1942 if 'I' in variant else (1943 if 'III' in variant else 1945)},
                    Generation = AircraftGeneration.WWII,
                    Role = AircraftRole.Bomber,
                    Description = "RAF's primary heavy bomber",
                    MaxSpeed = 454,
                    CruiseSpeed = 338,
                    MaxAltitude = 7315,
                    RateOfClimb = 3.8f,
                    Range = 4072,
                    ServiceCeiling = 7350,
                    Length = 21.18f,
                    Wingspan = 31.09f,
                    Height = 6.10f,
                    EmptyWeight = 16783,
                    MaxTakeoffWeight = 29484,
                    FuelCapacity = 9792,
                    Crew = 7,
                    ArmorRating = 70,
                    Maneuverability = 22,
                    Weapons = new List<WeaponSystem>
                    {{
                        new WeaponSystem {{ Name = ".303 Browning", Type = "Gun", Quantity = 8, Damage = 15, Range = 700, RateOfFire = 1200, Accuracy = 62 }},
                        new WeaponSystem {{ Name = "Bombs", Type = "Bomb", Quantity = 14, Damage = 1000, Range = 0, RateOfFire = 0, Accuracy = 65 }}
                    }},
                    PurchasePrice = 170000,
                    MaintenanceCost = 5600,
                    RepairCost = 17000,
                    UnlockLevel = 19
                }},''')
    
    return '\n'.join(aircraft_list)

# Generate the output
if __name__ == "__main__":
    print("Generating WWII Axis Fighters...")
    print(generate_wwii_axis_fighters())
    print("\n\nGenerating WWII Bombers...")
    print(generate_wwii_bombers())
