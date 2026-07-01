using System;
using TASK2.Presentation;
using TASK2.Services;
class Program
{
    static void Main(string[] args)
    {
        var authService = new AuthService();

        MainMenu mainMenu = new MainMenu(authService);
        mainMenu.Display();
    }
}