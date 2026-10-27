workspace "Sep3 Architecture" "Vores Sep3 Arkitektur diagram" {
    model {
        user = person "Rejse planlægger" "en bruger af Tagalong"
        user2 = person "Slut Bruger" "En bruger af Tagalong"

        sep3 = softwareSystem "Tagalong" "Rejseplænlægning og budgettering" {
            javaClient = container "Java Client" "En Java klient applikation" "Java"
            blazorClient = container "Blazor Client" "En Blazor klient applikation" "Blazor"
            javaService = container "Java Service" "En Java database proxy applikation" "Java"
            csharpService = container "C# Service" "En C# database proxy applikation og SignalR hub" "C#"
            databaseGateway = container "Database Gateway" "En database gateway applikation der bruger efCore" "C#"
           
            database = container "Database" "En database der bruges af Sep3 systemet" "SQL" {
                tags "Database"
            }

            user -> javaClient "Opdaterer budget"
            javaClient -> javaService "Kommunikerer med"
            user -> blazorClient "Interagerer med"
            blazorClient -> csharpService "Kommunikerer med" "Rest" "Rest"
            csharpService -> blazorClient "Sender opdateringer til" "SignalR"
            javaService -> csharpService "Kommunikerer med" "Rest" "Rest"
            javaService -> databaseGateway "Kommunikerer med" "Rest" "Rest"
            csharpService -> databaseGateway "Kommunikerer med" "Rest" "Rest"
            databaseGateway -> database "Kommunikerer med" "EfCore"
        }

        overview = softwareSystem "Sep3 System Overview" "Sep3 applikationer" {
            user2 -> sep3 "Bruger systemet via java klient"
            user2 -> sep3 "Bruger systemet via blazor klient"
            sep3 -> user2 "Viser data"
        }
    }

    views {
        
        systemlandscape "c1" {
            include sep3 user2
        }
        systemContext sep3 "c2" {
            include *
            autolayout
        }

        container sep3 "Sep3-System-Containers" {
            include *
        }

        container sep3 "Service-To-Service" {
            include *
            exclude javaClient blazorClient user
        }

        styles {
            element "Database" {
                shape cylinder
                background "#ff0000"
                color "#ffffff"
            }
            element "Person" {
                shape person
                background "#0000ff"
                color "#ffffff"
            }
            relationship "Relationship" {
                dashed false
            }
            relationship "Rest" {
                color "#00ff00"
                dashed true
            }
        }
    }
}