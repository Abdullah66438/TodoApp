// for AddScoped
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;   // required for AddScoped
using TodoApp.Core.Abstractions;
using TodoApp.Infrastructure.Repositories;
using TodoApp.Infrastructure.Storage;
using TodoApp.Web;

// You can keep or remove this next using With Marker.cs it will work either way.
// using TodoApp.Web;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// EITHER keep the using and this line:
builder.RootComponents.Add<App>("#app");

// OR, if you want to bypass any namespace confusion, fully-qualify App:
builder.RootComponents.Add<TodoApp.Web.App>("#app");

builder.RootComponents.Add<HeadOutlet>("head::after");

// DI: Infra + Repos
builder.Services.AddScoped<IndexedDbInterop>();
builder.Services.AddScoped<IAssigneeRepository, AssigneeRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

await builder.Build().RunAsync();
