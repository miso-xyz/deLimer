Imports System.Security.Cryptography
Imports System.Reflection
Imports System.Text
Imports System.IO
Imports System.Resources
Imports dnlib.DotNet

Module Module2
    Public inputFile
    Sub Main(ByVal args As String())
        Console.Title = "UnStub"
        If args.Count = 0 Then
            Console.WriteLine("Please set an input file")
            Console.ReadKey()
            End
        End If
        inputFile = args(1)
        Console.WriteLine("UnStub 1.0 by misonothx")
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
        Console.Write("Decryption")
            Try
            File.WriteAllBytes(args(0) & "-decoded.bin", decrypt(File.ReadAllBytes(args(0))))
                Console.ForegroundColor = ConsoleColor.Green
                Console.WriteLine(" Successful!")
                Console.ResetColor()
            Catch ex As Exception
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine(" Failed!")
                Console.ResetColor()
                Console.WriteLine(" (exception: " & ex.Message & ")")
            End Try
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
            For x = 0 To patchedApp.Types(1).Methods(1).Body.Instructions.Count - 1
                If patchedApp.Types(1).Methods(1).Body.Instructions(x).OpCode.ToString = "ldstr" Then
                    Return patchedApp.Types(1).Methods(1).Body.Instructions(x).Operand.ToString
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