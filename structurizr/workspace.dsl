workspace "Sep3 Architecture" "Vores Sep3 Arkitektur diagram" {
    model {
        user = person "User" "en bruger af Sep3 systemet"

        softwareSystem = softwareSystem "Sep3 System" "Sep3 systemet" {
            javaClient = container "Java Client" "En Java klient applikation" "Java"
            javaService = container "Java Service" "En Java database proxy applikation" "Java"
            blazorClient = container "Blazor Client" "En Blazor klient applikation" "Blazor"
            csharpService = container "C# Service" "En C# database proxy applikation og SignalR hub" "C#"
            databaseGateway = container "Database Gateway" "En database gateway applikation der bruger efCore" "C#"
           
            database = container "Database" "En database der bruges af Sep3 systemet" "SQL" {
                tags "Database"
            }

            user -> javaClient "Opdaterer budget"
            javaClient -> javaService "Kommunikerer med"
            user -> blazorClient "Interagerer med"
            blazorClient -> csharpService "Kommunikerer med"
            csharpService -> blazorClient "Sender opdateringer til via SignalR"
            javaService -> csharpService "Kommunikerer med"
            javaService -> databaseGateway "Kommunikerer med"
            csharpService -> databaseGateway "Kommunikerer med"
            databaseGateway -> database "Kommunikerer med"
        }
    }

    views {
        systemContext softwareSystem "Sep3-System" {
            include *
            autolayout
        }

        container softwareSystem "Sep3-System-Containers" {
            include *
            autolayout
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
        }
    }
}