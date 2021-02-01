Imports System.Security.Cryptography
Imports System.Reflection
Imports System.Text
Imports System.IO
Imports System.Resources
Imports dnlib.DotNet

Module Module1
    Public inputFile
    Sub Main(ByVal args As String())
        Console.Title = "deLimer"
        If args.Count = 0 Then
            Console.WriteLine("Please set an input file")
            Console.ReadKey()
            End
        End If
        inputFile = args(0)
        Console.WriteLine("deLimer 1.0 by misonothx")
        Console.WriteLine()
        Console.ForegroundColor = ConsoleColor.Yellow
        If getPassword() <> "???" Then
            Console.Write("Password: ")
            Console.ForegroundColor = ConsoleColor.Magenta
            Console.WriteLine(getPassword())
        Else
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("Failed to retrieve password, cannot continue")
            Console.ReadKey()
            End
        End If
        Console.ForegroundColor = ConsoleColor.Yellow
            Dim asm = Assembly.LoadFile(inputFile)
            Console.WriteLine("Decrypting resource...")
            If getResName() <> "???" Then
                Console.WriteLine("Resource Name: " & getResName())
        Else
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("Failed to retrieve resource name, cannot continue")
            Console.ReadKey()
            End
            End If
            If IO.Directory.Exists("deLimer") Then
                IO.Directory.CreateDirectory("deLimer")
            End If
        IO.Directory.CreateDirectory("deLimer\" & Path.GetFileName(args(0)))
            Console.Write("Extraction")
            For x2 = 0 To asm.GetManifestResourceNames.Count - 1
                Try
                    Dim rm As New ResourceManager(asm.GetManifestResourceNames(x2).Replace(".resources", Nothing), asm)
                    File.WriteAllBytes("deLimer\" & Path.GetFileName(args(0)) & "\" & getResName() & ".bin", decrypt(rm.GetObject(getResName())))
                    inputFile = "deLimer\" & Path.GetFileName(args(0)) & "\" & getResName() & ".bin"
                    Console.ForegroundColor = ConsoleColor.Green
                    Console.WriteLine(" Successful!")
                    Console.ResetColor()
                Catch ex As Exception
                    Console.ForegroundColor = ConsoleColor.Red
                    Console.WriteLine(" Failed!")
                    Console.ResetColor()
                    Console.WriteLine(" (exception: " & ex.Message & ")")
                End Try
        Next
        Console.ResetColor()
        Console.WriteLine("Finished.")
        Console.ReadKey()
        'Dim resources = Assembly.LoadFile(args(1)).GetManifestResourceStream()
    End Sub
    Function getPassword() As String
        Try
            Dim patchedApp As dnlib.DotNet.ModuleDef = dnlib.DotNet.ModuleDefMD.Load(inputFile)
            For x = 0 To patchedApp.Types(2).Methods(2).Body.Instructions.Count - 1
                If patchedApp.Types(2).Methods(2).Body.Instructions(x).OpCode.ToString = "ldstr" Then
                    Return patchedApp.Types(2).Methods(2).Body.Instructions(x).Operand.ToString
                End If
            Next
        Catch ex As Exception
        End Try
        Return "???"
    End Function

    Function getResName() As String
        Try
            Dim patchedApp As dnlib.DotNet.ModuleDef = dnlib.DotNet.ModuleDefMD.Load(inputFile)
            For x = 0 To patchedApp.Types(2).Methods(1).Body.Instructions.Count - 1
                If patchedApp.Types(2).Methods(1).Body.Instructions(x).OpCode.ToString = "ldstr" Then
                    Return patchedApp.Types(2).Methods(1).Body.Instructions(x).Operand.ToString
                End If
            Next
        Catch ex As Exception
        End Try
        Return "???"
    End Function

    Private Function decrypt(ByVal bytesToBeDecrypted As Byte()) As Byte()
        Dim result As Byte() = Nothing
        Dim salt As Byte() = New Byte() {1, 2, 3, 4, 5, 6, 7, 8}
        Using memoryStream As MemoryStream = New MemoryStream()
            Using rijndaelManaged As RijndaelManaged = New RijndaelManaged()
                rijndaelManaged.KeySize = 256
                rijndaelManaged.BlockSize = 128
                Dim bytes As Byte() = Encoding.UTF8.GetBytes("kgzaj12efkj")
                Dim rfc2898DeriveBytes As Rfc2898DeriveBytes = New Rfc2898DeriveBytes(bytes, salt, 1000)
                rijndaelManaged.Key = rfc2898DeriveBytes.GetBytes(rijndaelManaged.KeySize / 8)
                rijndaelManaged.IV = rfc2898DeriveBytes.GetBytes(rijndaelManaged.BlockSize / 8)
                rijndaelManaged.Mode = CipherMode.CBC
                Using cryptoStream As CryptoStream = New CryptoStream(memoryStream, rijndaelManaged.CreateDecryptor(), CryptoStreamMode.Write)
                    cryptoStream.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length)
                    cryptoStream.Close()
                End Using
                result = memoryStream.ToArray()
            End Using
        End Using
        Return result
    End Function
End Module