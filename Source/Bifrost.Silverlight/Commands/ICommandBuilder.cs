﻿namespace Bifrost.Commands
{
    /// <summary>
    /// Defines a builder for building <see cref="ICommand">commands</see>
    /// </summary>
    public interface ICommandBuilder
    {
        /// <summary>
        /// Gets or sets the name of the <see cref="ICommand"/>being built
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the default parameters for the <see cref="ICommand"/>
        /// </summary>
        dynamic Parameters { get; set; }

        /// <summary>
        /// Get an instance of the <see cref="ICommand"/>
        /// </summary>
        /// <returns>An instance of the <see cref="ICommand"/></returns>
        ICommand GetInstance();
    }
}